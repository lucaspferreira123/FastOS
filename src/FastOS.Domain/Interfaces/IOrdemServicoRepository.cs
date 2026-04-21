using FastOS.Domain.Entities;
using FastOS.Domain.ValueObjects;

namespace FastOS.Domain.Interfaces;

public interface IOrdemServicoRepository : IBaseRepository<OrdemServicoViewModel>
{
    Task<OrdemServicoViewModel> CadastrarOrdem(OrdemServicoViewModel ordem);
    Task<List<OrdemServicoDto>> ObterTodasOrdens();
    Task<OrdemServicoDto?> ObterOrdemDto(int idOrdem);
    Task<OrdemServicoViewModel> AlterarOrdemServico(OrdemServicoViewModel model);
}
