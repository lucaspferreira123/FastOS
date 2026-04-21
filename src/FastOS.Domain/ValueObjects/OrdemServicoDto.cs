namespace FastOS.Domain.ValueObjects;

public class OrdemServicoDto
{
    public int idOrdemServico { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public int idCliente { get; set; }
    public string StatusDescricao { get; set; } = string.Empty;
    public int idStatus { get; set; }
    public bool Pago { get; set; }
    public string DescricaoServico { get; set; } = string.Empty;
    public DateTime DataAbertura { get; set; }
    public DateTime PrevisaoEntrega { get; set; }
}
