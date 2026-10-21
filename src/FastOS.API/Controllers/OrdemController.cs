using FastOS.Application.Services;
using FastOS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FastOS.API.Controllers;

public class OrdemController : Controller
{
    private const long MaxPdfBytes = 20 * 1024 * 1024;
    private readonly OrdemServicoBusiness _ordemBusiness;

    public OrdemController(OrdemServicoBusiness ordemBusiness)
    {
        _ordemBusiness = ordemBusiness;
    }

    [HttpPost]
    [Route("Ordem/CadastrarOrdem")]
    [RequestSizeLimit(MaxPdfBytes + 1024 * 1024)]
    public async Task<IActionResult> CadastrarOrdem([FromForm] OrdemServicoEntity ordem, [FromForm] IFormFile? pdfFile, CancellationToken cancellationToken)
    {
        try
        {
            if (pdfFile is not null)
            {
                await using var validationStream = pdfFile.OpenReadStream();
                var validationError = await PdfUploadValidator.ValidarAsync(pdfFile.FileName, pdfFile.Length, validationStream, MaxPdfBytes, cancellationToken);
                if (validationError is not null) return BadRequest(new { message = validationError });
            }

            await using var pdfStream = pdfFile?.OpenReadStream();
            var ordemCadastrada = await _ordemBusiness.CadastrarOrdem(ordem, pdfStream, cancellationToken);

            return Ok(new
            {
                id = ordemCadastrada.idOrdemServico,
                hasReportPdf = ordemCadastrada.RelatorioPdfFileName is not null,
                mensagem = "Criado com sucesso"
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Não foi possível salvar a ordem e o relatório anexado. Nenhuma ordem foi criada." });
        }
    }

    [HttpGet]
    [Route("Ordem/RelatorioPdf/{idOrdem:int}")]
    [Authorize(Roles = "Admin,Tecnico")]
    public async Task<IActionResult> RelatorioPdf(int idOrdem, CancellationToken cancellationToken)
    {
        var stream = await _ordemBusiness.ObterRelatorioPdf(idOrdem, cancellationToken);
        if (stream is null) return NotFound(new { message = "Relatório PDF não encontrado para esta ordem." });
        return File(stream, "application/pdf", $"relatorio-os-{idOrdem}.pdf", enableRangeProcessing: true);
    }


    [HttpGet]
    [Route("Ordem/ObterOrdens")]
    public async Task<IActionResult> ObterTodasOrdens()
    {
        try
        {
            var ordens = await _ordemBusiness.ObterTodasOrdens();
            return Ok(ordens);
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocorreu um erro interno.");
        }
    }

    [HttpGet]
    [Route("Ordem/ObterOrdem/{idOrdem}")]
    public async Task<IActionResult> ObterOrdem(int idOrdem)
    {
        try
        {
            var ordem = await _ordemBusiness.ObterOrdemDto(idOrdem);
            return Ok(ordem);
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocorreu um erro interno.");
        }
    }

    [HttpPut]
    [Route("Ordem/AlterarOrdem")]
    public async Task<IActionResult> AlterarOrdem([FromBody] OrdemServicoEntity model)
    {
        try
        {
            var ordem = await _ordemBusiness.AlterarOrdemServico(model);
            return Ok(ordem);
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocorreu um erro interno.");
        }
    }

    [HttpPut]
    [Route("Ordem/AlterarStatusOrdem/{idOrdem}/{idStatus}")]
    public async Task<IActionResult> AlterarStatusOrdem(int idOrdem, int idStatus)
    {
        try
        {
            await _ordemBusiness.AlterarStatusOrdem(idOrdem, idStatus);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocorreu um erro interno.");
        }
    }
}
