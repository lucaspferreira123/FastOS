using TesteMVC.Dto.TesteMVC.Dto;
using TesteMVC.Models;

namespace TesteMVC.IRepository
{
    public interface IItemOrdemServicoRepository:IBaseRepository<ItemOrdemServicoViewModel>
    {
        Task<List<ItemOrdemServicoViewModel>> AlterarItensOrdemServico(List<ItemOrdemServicoViewModel> itens);
        Task<List<ItensOrdemServicoDto>> ObterItensOrdemServico(int idOrdem);
    }
}
