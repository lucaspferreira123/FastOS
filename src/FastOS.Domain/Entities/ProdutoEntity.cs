using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastOS.Domain.Entities;

public class ProdutoEntity
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "varchar(300)")]
    [MaxLength(300)]
    public string Descricao { get; set; } = string.Empty;

    [Column(TypeName = "varchar(20)")]
    [MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;

    public decimal ValorCusto { get; set; }
    public decimal ValorVenda { get; set; }
    public decimal Quantidade { get; set; }
    public decimal QuantidadeMinimaEstoque { get; set; }
    public bool Excluido { get; set; }
}
