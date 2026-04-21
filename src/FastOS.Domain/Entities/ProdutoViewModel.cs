using System.ComponentModel.DataAnnotations;

namespace FastOS.Domain.Entities;

public class ProdutoViewModel
{
    [Key]
    public int idProduto { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public int QuantidadeTotal { get; set; }
    public string Marca { get; set; } = string.Empty;
}
