using System.Security.Claims;
using System.Text.Json;
using FastOS.Application.Services;
using FastOS.Domain.Entities;
using FastOS.Domain.Interfaces;
using FastOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastOS.API.Controllers;

[Authorize(Roles = "Admin,Tecnico")]
[Route("Diagnostico")]
public sealed class DiagnosticoController : Controller
{
    private const long MaxJsonBytes = 10 * 1024 * 1024;
    private const long MaxPdfBytes = 20 * 1024 * 1024;
    private readonly AppDbContext _db;
    private readonly IRelatorioPdfStorage _pdfStorage;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DiagnosticoController> _logger;

    public DiagnosticoController(AppDbContext db, IRelatorioPdfStorage pdfStorage, IWebHostEnvironment environment, ILogger<DiagnosticoController> logger)
    { _db = db; _pdfStorage = pdfStorage; _environment = environment; _logger = logger; }

    [HttpPost("Preview")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxJsonBytes)]
    public async Task<IActionResult> Preview([FromForm] IFormFile? jsonFile, CancellationToken token)
    {
        var parsed = await ReadDiagnostic(jsonFile, token);
        if (parsed.Error is not null) return BadRequest(new { message = parsed.Error });
        var root = parsed.Root!.Value;
        var id = root.GetProperty("diagnosticId").GetGuid();
        if (await _db.DiagnosticImports.AnyAsync(x => x.DiagnosticId == id, token))
            return Conflict(new { message = "Este diagnosticId já foi importado." });
        var computer = root.GetProperty("computer");
        var hardware = root.GetProperty("hardware");
        var health = root.GetProperty("health");
        return Ok(new
        {
            diagnosticId = id,
            generatedAt = root.GetProperty("generatedAt").GetDateTime(),
            computerName = ReadString(computer, "hostname"),
            operatingSystem = ReadString(root.GetProperty("operatingSystem"), "name"),
            processor = ReadString(hardware.GetProperty("processor"), "name"),
            memoryGb = ReadNumber(hardware.GetProperty("memory"), "totalGb"),
            hardware = hardware.Clone(),
            operatingSystemDetails = root.GetProperty("operatingSystem").Clone(),
            summary = ReadString(root, "summary"),
            status = ReadString(health, "status"),
            alerts = health.GetProperty("alerts").Clone(),
            warnings = root.GetProperty("warnings").Clone()
        });
    }

    [HttpPost("Importar/{idOrdem:int}")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxJsonBytes + MaxPdfBytes)]
    public async Task<IActionResult> Importar(int idOrdem, [FromForm] IFormFile? jsonFile, [FromForm] IFormFile? pdfFile, CancellationToken token)
    {
        var parsed = await ReadDiagnostic(jsonFile, token);
        if (parsed.Error is not null) return BadRequest(new { message = parsed.Error });
        if (pdfFile is not null && (pdfFile.Length <= 5 || pdfFile.Length > MaxPdfBytes || !await HasPdfSignature(pdfFile, token)))
            return BadRequest(new { message = "O PDF é inválido ou excede o limite de 20 MB." });

        var root = parsed.Root!.Value;
        var id = root.GetProperty("diagnosticId").GetGuid();
        if (!await _db.OrdensServico.AnyAsync(x => x.idOrdemServico == idOrdem, token))
            return NotFound(new { message = "Ordem de serviço não encontrada." });
        if (await _db.DiagnosticImports.AnyAsync(x => x.DiagnosticId == id, token))
            return Conflict(new { message = "Este diagnosticId já foi importado." });

        string? storedPdf = null;
        try
        {
            if (pdfFile is not null)
            {
                await using var pdfStream = pdfFile.OpenReadStream();
                storedPdf = await _pdfStorage.SalvarAsync(pdfStream, token);
            }

            var entity = new DiagnosticImportEntity
            {
                DiagnosticId = id,
                OrdemServicoId = idOrdem,
                GeneratedAt = root.GetProperty("generatedAt").GetDateTime().ToUniversalTime(),
                ImportedAt = DateTime.UtcNow,
                ComputerName = Limit(ReadString(root.GetProperty("computer"), "hostname") ?? "Não informado", 200),
                Summary = Limit(ReadString(root, "summary") ?? "", 2000),
                DiagnosticJson = parsed.Json!,
                PdfFileName = storedPdf,
                ImportedByUserId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : null
            };
            _db.DiagnosticImports.Add(entity);
            await _db.SaveChangesAsync(token);
            return Ok(new { id = entity.Id, message = "Diagnóstico associado à ordem de serviço." });
        }
        catch (DbUpdateException ex)
        {
            if (storedPdf is not null) await _pdfStorage.RemoverAsync(storedPdf, CancellationToken.None);
            _logger.LogWarning(ex, "Duplicidade ou falha de persistência ao importar diagnóstico {DiagnosticId}", id);
            if (await _db.DiagnosticImports.AnyAsync(x => x.DiagnosticId == id, token))
                return Conflict(new { message = "Este diagnosticId já foi importado." });
            return StatusCode(500, new { message = "Não foi possível persistir o diagnóstico." });
        }
        catch (Exception ex)
        {
            if (storedPdf is not null) await _pdfStorage.RemoverAsync(storedPdf, CancellationToken.None);
            _logger.LogError(ex, "Falha ao importar diagnóstico {DiagnosticId}", id);
            return StatusCode(500, new { message = "Não foi possível concluir a importação." });
        }
    }

    [HttpGet("PorOrdem/{idOrdem:int}")]
    public async Task<IActionResult> PorOrdem(int idOrdem, CancellationToken token)
    {
        var items = await _db.DiagnosticImports.AsNoTracking().Where(x => x.OrdemServicoId == idOrdem)
            .OrderByDescending(x => x.GeneratedAt).Select(x => new { x.Id, x.DiagnosticId, x.GeneratedAt, x.ImportedAt, x.ComputerName, x.Summary, hasPdf = x.PdfFileName != null }).ToListAsync(token);
        return Ok(items);
    }

    [HttpGet("Pdf/{id:int}")]
    public async Task<IActionResult> Pdf(int id, CancellationToken token)
    {
        var import = await _db.DiagnosticImports.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, token);
        if (import is null) return NotFound();
        if (string.IsNullOrWhiteSpace(import.PdfFileName) || Path.GetFileName(import.PdfFileName) != import.PdfFileName) return NotFound();
        var stream = await _pdfStorage.AbrirAsync(import.PdfFileName, token);
        if (stream is not null) return File(stream, "application/pdf", $"diagnostico-{import.DiagnosticId:N}.pdf", enableRangeProcessing: true);

        // Importações anteriores à configuração comum guardavam PDFs neste diretório.
        var legacyFolder = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "App_Data", "diagnostics"));
        var legacyPath = Path.GetFullPath(Path.Combine(legacyFolder, import.PdfFileName));
        if (!legacyPath.StartsWith(legacyFolder + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) || !System.IO.File.Exists(legacyPath)) return NotFound();
        return PhysicalFile(legacyPath, "application/pdf", $"diagnostico-{import.DiagnosticId:N}.pdf", enableRangeProcessing: true);
    }

    [HttpGet("Detalhe/{id:int}")]
    public async Task<IActionResult> Detalhe(int id, CancellationToken token)
    {
        var import = await _db.DiagnosticImports.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, token);
        if (import is null) return NotFound();
        try { return Content(import.DiagnosticJson, "application/json; charset=utf-8"); }
        catch (Exception ex) { _logger.LogWarning(ex, "JSON persistido inválido para diagnóstico {Id}", id); return StatusCode(500, new { message = "Os dados persistidos não puderam ser lidos." }); }
    }

    private static async Task<(JsonElement? Root, string? Json, string? Error)> ReadDiagnostic(IFormFile? file, CancellationToken token)
    {
        if (file is null || file.Length <= 0) return (null, null, "Selecione um arquivo JSON.");
        if (file.Length > MaxJsonBytes) return (null, null, "O JSON excede o limite de 10 MB.");
        try
        {
            await using var stream = file.OpenReadStream();
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory, token);
            var json = new System.Text.UTF8Encoding(false, true).GetString(memory.ToArray());
            var validation = DiagnosticImportValidator.Validate(json);
            return validation.IsValid ? (validation.Root, validation.Json, null) : (null, null, validation.Error);
        }
        catch (System.Text.DecoderFallbackException) { return (null, null, "O arquivo não contém texto UTF-8 válido."); }
        catch (JsonException) { return (null, null, "JSON malformado ou com estrutura excessivamente profunda."); }
    }

    private static string? ReadString(JsonElement element, string name) => element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;
    private static string Limit(string value, int maxLength) => value.Length <= maxLength ? value : value[..maxLength];
    private static double? ReadNumber(JsonElement element, string name) => element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out var number) ? number : null;
    private static async Task<bool> HasPdfSignature(IFormFile file, CancellationToken token)
    {
        await using var stream = file.OpenReadStream();
        var header = new byte[5];
        return await stream.ReadAsync(header.AsMemory(), token) == header.Length && header.AsSpan().SequenceEqual("%PDF-"u8);
    }
}

