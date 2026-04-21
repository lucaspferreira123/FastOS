namespace FastOS.Domain.ValueObjects;

public class ItensOrdemServicoDto
{
    public int idOrdemServico { get; set; }
    public int idItemOrdemServico { get; set; }
    public int idProduto { get; set; }
    public string nomeProduto { get; set; } = string.Empty;
    public string quantidade { get; set; } = string.Empty;
    public string DescricaoServico { get; set; } = string.Empty;
    public decimal valorUnitario { get; set; }
}
