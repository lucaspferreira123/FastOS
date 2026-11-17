using System.ComponentModel.DataAnnotations;

namespace FastOS.Domain.Entities;

public class DiagnosticImportEntity
{
    [Key] public int Id { get; set; }
    public Guid DiagnosticId { get; set; }
    public int OrdemServicoId { get; set; }
    public DateTime GeneratedAt { get; set; }
    public DateTime ImportedAt { get; set; }
    [MaxLength(200)] public string ComputerName { get; set; } = "";
    [MaxLength(2000)] public string Summary { get; set; } = "";
    public string DiagnosticJson { get; set; } = "";
    [MaxLength(500)] public string? PdfFileName { get; set; }
    public int? ImportedByUserId { get; set; }
    public OrdemServicoEntity OrdemServico { get; set; } = null!;
}
