namespace FastOS.Domain.ValueObjects;

public class ClientePerfilDto
{
    public int IdCliente { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Endereco { get; set; }
    public string TipoCliente { get; set; } = string.Empty;
    public string? Documento { get; set; }
    public bool Ativo { get; set; }

    // Indicadores
    public int TotalOS { get; set; }
    public int OSAbertas { get; set; }
    public int OSConcluidas { get; set; }
    public int OSAtrasadas { get; set; }
    public decimal TotalGasto { get; set; }
    public decimal TicketMedio { get; set; }

    // Histórico
    public List<OrdemServicoDto> Ordens { get; set; } = [];
    public List<string> Equipamentos { get; set; } = [];
}
