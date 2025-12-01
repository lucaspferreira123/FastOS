using MeuProjeto.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TesteMVC.Dto;
using TesteMVC.IRepository;
using TesteMVC.Models;

namespace MeuProjeto.Business
{
    public class OrdemServicoBusiness
    {
        private readonly IOrdemServicoRepository _repository;

        public OrdemServicoBusiness(IOrdemServicoRepository repository)
        {
            _repository = repository;
        }

        public async Task<OrdemServicoViewModel> CadastrarOrdem(OrdemServicoViewModel ordem)
        {
            try
            {
                if (ordem == null)
                {
                    throw new ArgumentException("Não foi possivel cadastrar a ordem.");
                }

                var ordemCadastrada = await _repository.CadastrarOrdem(ordem);

                return ordemCadastrada;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao cadastrar a ordem", ex);
            }
        }
        
        public async Task<List<OrdemServicoDto>> ObterTodasOrdens()
        {
            try
            {
                var ordens = await _repository.ObterTodasOrdens();

                return ordens;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter as ordens", ex);
            }
        }

        public async Task<OrdemServicoDto> ObterOrdemDto(int idOrdem)
        {
            try
            {
                var ordem = await _repository.ObterOrdemDto(idOrdem);

                return ordem;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter a ordem", ex);
            }
        }

        public async Task<OrdemServicoViewModel> AlterarOrdemServico(OrdemServicoViewModel model)
        {
            try
            {
                var ordem = await _repository.AlterarOrdemServico(model);

                return ordem;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao alterar a ordem", ex);
            }
        }

        public async Task AlterarStatusOrdem(int idOrdem, int idStatus)
        {
            try
            {
                var ordem = await _repository.GetByIdAsync(idOrdem);

                if (ordem == null)
                    return; 

                if (idStatus == 5)
                {
                    ordem.Pago = true;
                }
                    ordem.idStatus = idStatus;

                await _repository.UpdateAsync(ordem);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao alterar a ordem", ex);
            }
        }

    }
}