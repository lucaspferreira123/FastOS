using System.ComponentModel.DataAnnotations;

namespace FastOS.Domain.Entities;

public class OrdemServicoEntity
{
    [Key]
    public int idOrdemServico { get; set; }
    public int idCliente { get; set; }
    public bool Pago { get; set; }
    public int idStatus { get; set; }
    public string DescricaoServico { get; set; } = string.Empty;
    public DateTime DataAbertura { get; set; }
    public DateTime PrevisaoEntrega { get; set; }
    public ICollection<ItemOrdemServicoEntity> Itens { get; set; } = [];
    public OrcamentoEntity? Orcamento { get; set; }
}
