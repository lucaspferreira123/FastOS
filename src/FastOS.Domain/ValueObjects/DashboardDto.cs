namespace FastOS.Domain.ValueObjects;

public class DashboardDto
{
    public int TotalOS { get; set; }
    public int OSEmAndamento { get; set; }
    public int OSConcluidas { get; set; }
    public int OSAtrasadas { get; set; }
    public double TempoMedioResolucaoDias { get; set; }

    public List<OsPorStatusDto> OsPorStatus { get; set; } = [];
    public List<OsPorMesDto> OsPorMes { get; set; } = [];
    public List<OsPorClienteDto> OsPorCliente { get; set; } = [];
    public List<OsPorDiaSemanaDto> OsPorDiaSemana { get; set; } = [];
    public List<OsAbertasVsConcluidas> OsAbertasVsConcluidas { get; set; } = [];
    public List<TempoMedioPorMesDto> TempoMedioPorMes { get; set; } = [];
    public List<OrdemServicoDto> UltimasOrdens { get; set; } = [];
}

public class OsPorStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class OsPorMesDto
{
    public string Mes { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class OsPorClienteDto
{
    public string Cliente { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class OsPorDiaSemanaDto
{
    public string DiaSemana { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class OsAbertasVsConcluidas
{
    public string Mes { get; set; } = string.Empty;
    public int Abertas { get; set; }
    public int Concluidas { get; set; }
}

public class TempoMedioPorMesDto
{
    public string Mes { get; set; } = string.Empty;
    public double TempoMedioDias { get; set; }
}
