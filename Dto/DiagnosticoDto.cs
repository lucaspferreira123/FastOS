public class DiagnosticoDto
{
    public DateTime Data { get; set; }
    public MaquinaDto Maquina { get; set; }
}

public class MaquinaDto
{
    public string Hostname { get; set; }
    public string SistemaOperacional { get; set; }
    public string Arquitetura { get; set; }
    public string Fabricante { get; set; }
    public string Modelo { get; set; }
}
