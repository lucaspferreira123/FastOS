using System.ComponentModel.DataAnnotations;
using TesteMVC.Models;

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
    public string FormaPagamento { get; set; }
    public OrdemServicoViewModel OrdemServico { get; set; }
}
