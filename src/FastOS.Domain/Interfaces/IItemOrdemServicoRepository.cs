using FastOS.Domain.Entities;
using FastOS.Domain.ValueObjects;

namespace FastOS.Domain.Interfaces;

public interface IItemOrdemServicoRepository : IBaseRepository<ItemOrdemServicoViewModel>
{
    Task<List<ItemOrdemServicoViewModel>> AlterarItensOrdemServico(List<ItemOrdemServicoViewModel> itens);
    Task<List<ItensOrdemServicoDto>> ObterItensOrdemServico(int idOrdem);
}
