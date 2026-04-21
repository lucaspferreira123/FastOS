using FastOS.Domain.Entities;

namespace FastOS.Domain.ValueObjects;

public class RelatorioOrcamentoDto
{
    public OrdemServicoDto OrdemServico { get; set; } = null!;
    public OrcamentoViewModel Orcamento { get; set; } = null!;
    public List<ItensOrdemServicoDto> Itens { get; set; } = [];
}
