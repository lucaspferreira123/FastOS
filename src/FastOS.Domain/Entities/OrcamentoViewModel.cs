using System.ComponentModel.DataAnnotations;

namespace FastOS.Domain.Entities;

public class OrcamentoViewModel
{
    [Key]
    public int idOrcamento { get; set; }
    public int idOrdemServico { get; set; }
    public decimal MaoDeObra { get; set; }
    public decimal Materiais { get; set; }
    public decimal Desconto { get; set; }
    public decimal TaxasExtras { get; set; }
    public decimal ValorFinal { get; set; }
    public string FormaPagamento { get; set; } = string.Empty;
    public OrdemServicoViewModel OrdemServico { get; set; } = null!;
}
