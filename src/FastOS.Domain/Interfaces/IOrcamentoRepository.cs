using FastOS.Domain.Entities;

namespace FastOS.Domain.Interfaces;

public interface IOrcamentoRepository : IBaseRepository<OrcamentoViewModel>
{
    Task<OrcamentoViewModel> AlterarOrcamento(OrcamentoViewModel model);
    Task<OrcamentoViewModel?> ObterOrcamentoPorOrdemServico(int idOrdemServico);
}
