using FastOS.Domain.Entities;

namespace FastOS.Domain.Interfaces;

public interface IOrcamentoRepository : IBaseRepository<OrcamentoEntity>
{
    Task<OrcamentoEntity> AlterarOrcamento(OrcamentoEntity model);
    Task<OrcamentoEntity?> ObterOrcamentoPorOrdemServico(int idOrdemServico);
}
