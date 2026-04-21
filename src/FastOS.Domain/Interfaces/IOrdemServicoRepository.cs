using FastOS.Domain.Entities;
using FastOS.Domain.ValueObjects;

namespace FastOS.Domain.Interfaces;

public interface IOrdemServicoRepository : IBaseRepository<OrdemServicoEntity>
{
    Task<OrdemServicoEntity> CadastrarOrdem(OrdemServicoEntity ordem);
    Task<List<OrdemServicoDto>> ObterTodasOrdens();
    Task<OrdemServicoDto?> ObterOrdemDto(int idOrdem);
    Task<OrdemServicoEntity> AlterarOrdemServico(OrdemServicoEntity model);
}
