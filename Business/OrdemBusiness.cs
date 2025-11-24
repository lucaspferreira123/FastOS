using MeuProjeto.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TesteMVC.Dto;
using TesteMVC.Models;

namespace MeuProjeto.Business
{
    public class OrdemBusiness
    {
        private readonly OrdemRepository _repository;

        public OrdemBusiness(OrdemRepository repository)
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
    }
}