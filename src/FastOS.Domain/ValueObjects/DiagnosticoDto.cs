namespace FastOS.Domain.ValueObjects;

public class DiagnosticoDto
{
    public DateTime Data { get; set; }
    public MaquinaDto Maquina { get; set; } = null!;
}

public class MaquinaDto
{
    public string Hostname { get; set; } = string.Empty;
    public string SistemaOperacional { get; set; } = string.Empty;
    public string Arquitetura { get; set; } = string.Empty;
    public string Fabricante { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
}
