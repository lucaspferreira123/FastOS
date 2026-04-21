using System.ComponentModel.DataAnnotations;

namespace FastOS.Domain.Entities;

public class ItemOrdemServicoViewModel
{
    [Key]
    public int idItemOrdem { get; set; }
    public int idOrdemServico { get; set; }
    public int idProduto { get; set; }
    public DateTime DataPedido { get; set; }
    public DateTime? DataRealizado { get; set; }
    public int Quantidade { get; set; }
    public OrdemServicoViewModel OrdemServico { get; set; } = null!;
}
