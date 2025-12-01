using TesteMVC.Dto;
using TesteMVC.Models;

namespace TesteMVC.IRepository
{
    public interface IOrcamentoRepository : IBaseRepository<OrcamentoViewModel>
    {
        Task<OrcamentoViewModel> AlterarOrcamento(OrcamentoViewModel model);
        Task<OrcamentoViewModel> ObterOrcamentoPorOrdemServico(int idOrdemServico);
    }
}
