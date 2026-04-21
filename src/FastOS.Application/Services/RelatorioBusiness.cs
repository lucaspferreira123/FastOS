using FastOS.Infrastructure.Repositories;
using FastOS.Domain.ValueObjects;
using FastOS.Domain.Interfaces;
using FastOS.Domain.Entities;

namespace FastOS.Application.Services
{
    public class RelatorioBusiness
    {
        private readonly OrdemServicoBusiness _ordemServicoBusiness;
        private readonly OrcamentoBusiness _orcamentoBusiness;
        private readonly IItemOrdemServicoRepository _itemOrdemServicoRepository;

        public RelatorioBusiness(OrdemServicoBusiness ordemServicoBusiness, IItemOrdemServicoRepository itemOrdemServicoRepository, OrcamentoBusiness orcamentoBusiness)
        {
            _ordemServicoBusiness = ordemServicoBusiness;
            _itemOrdemServicoRepository = itemOrdemServicoRepository;
            _orcamentoBusiness = orcamentoBusiness;
        }

        public async Task<RelatorioOrcamentoDto> PopularRelatorioOrcamento(int idOrdem)
        {
            try
            {
                if (idOrdem == 0)
                {
                    throw new ArgumentException("Não foi possível imprimir o orcamento.");
                }

                var ordem = await _ordemServicoBusiness.ObterOrdemDto(idOrdem);

                var itens = await _itemOrdemServicoRepository.ObterItensOrdemServico(idOrdem);

                var orcamento = await _orcamentoBusiness.ObterOrcamento(idOrdem);

                var itensOrdem = itens.Where(i => i.idOrdemServico == idOrdem).ToList();

                var relatorioDto = new RelatorioOrcamentoDto
                {
                    OrdemServico = ordem,
                    Itens = itensOrdem,
                    Orcamento = orcamento 
                };

                return relatorioDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao imprimir o orcamento", ex);
            }
        }
    }
}

