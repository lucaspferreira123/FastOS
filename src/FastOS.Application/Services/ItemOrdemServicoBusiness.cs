using FastOS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using FastOS.Domain.ValueObjects;
using FastOS.Domain.ValueObjects;
using FastOS.Domain.Interfaces;
using FastOS.Domain.Entities;

namespace FastOS.Application.Services
{
    public class ItemOrdemServicoBusiness
    {
        private readonly IItemOrdemServicoRepository _repository;

        public ItemOrdemServicoBusiness(IItemOrdemServicoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ItemOrdemServicoEntity>> AlterarItensOrdemServico(List<ItemOrdemServicoEntity> itens)
        {
            try
            {
                if (!itens.Any())
                {
                    throw new ArgumentException("Não foi possivel salvar os itens da ordem.");
                }

                var cadastrarItens = await _repository.AlterarItensOrdemServico(itens);

                return cadastrarItens;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao salvar os itens da ordem", ex);
            }
        }

        public async Task<List<ItensOrdemServicoDto>> ObterItensOrdemServico(int idOrdem)
        {
            try
            {
                var itens = await _repository.ObterItensOrdemServico(idOrdem);

                return itens;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter os itens da ordem", ex);
            }
        }
    }
}
