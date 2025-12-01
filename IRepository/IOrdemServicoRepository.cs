using TesteMVC.Dto;
using TesteMVC.Models;

namespace TesteMVC.IRepository
{
    public interface IOrdemServicoRepository: IBaseRepository<OrdemServicoViewModel>
    {
        Task<OrdemServicoViewModel> CadastrarOrdem(OrdemServicoViewModel ordem);
        Task<List<OrdemServicoDto>> ObterTodasOrdens();
        Task<OrdemServicoDto> ObterOrdemDto(int idOrdem);
        Task<OrdemServicoViewModel> AlterarOrdemServico(OrdemServicoViewModel model);
    }
}
