using FastOS.Domain.Entities;
using FastOS.Domain.ValueObjects;

namespace FastOS.Domain.Interfaces;

public interface IItemOrdemServicoRepository : IBaseRepository<ItemOrdemServicoEntity>
{
    Task<List<ItemOrdemServicoEntity>> AlterarItensOrdemServico(List<ItemOrdemServicoEntity> itens);
    Task<List<ItensOrdemServicoDto>> ObterItensOrdemServico(int idOrdem);
}
