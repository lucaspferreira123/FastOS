using MeuProjeto.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TesteMVC.Dto;
using TesteMVC.Dto.TesteMVC.Dto;
using TesteMVC.IRepository;
using TesteMVC.Models;

namespace MeuProjeto.Business
{
    public class ItemOrdemServicoBusiness
    {
        private readonly IItemOrdemServicoRepository _repository;

        public ItemOrdemServicoBusiness(IItemOrdemServicoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ItemOrdemServicoViewModel>> AlterarItensOrdemServico(List<ItemOrdemServicoViewModel> itens)
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