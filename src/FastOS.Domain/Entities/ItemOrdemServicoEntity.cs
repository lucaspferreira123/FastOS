using System.ComponentModel.DataAnnotations;

namespace FastOS.Domain.Entities;

public class ItemOrdemServicoEntity
{
    [Key]
    public int idItemOrdem { get; set; }
    public int idOrdemServico { get; set; }
    public int idProduto { get; set; }
    public DateTime DataPedido { get; set; }
    public DateTime? DataRealizado { get; set; }
    public int Quantidade { get; set; }
    public OrdemServicoEntity OrdemServico { get; set; } = null!;
}
