using MeuProjeto.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TesteMVC.Dto;
using TesteMVC.IRepository;
using TesteMVC.Models;

namespace MeuProjeto.Business
{
    public class OrcamentoBusiness
    {
        private readonly IOrcamentoRepository _repository;

        public OrcamentoBusiness(IOrcamentoRepository repository)
        {
            _repository = repository;
        }

        public async Task<OrcamentoViewModel> AlterarOrcamento(OrcamentoViewModel orcamento)
        {
            try
            {

                var orcamentoAntigo = ObterOrcamento(orcamento.idOrdemServico).Result;

                var orcamentoNovo = new OrcamentoViewModel{};

                if (orcamentoAntigo == null)
                {
                    orcamentoNovo = await _repository.AddAsync(orcamento);
                } else {
                    orcamentoNovo = await _repository.AlterarOrcamento(orcamento);
                }

                return orcamentoNovo;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao cadastrar a ordem", ex);
            }
        }

        public async Task<OrcamentoViewModel> ObterOrcamento(int idOrdem)
        {
            try
            {
                var orcamento = await _repository.ObterOrcamentoPorOrdemServico(idOrdem);

                return orcamento;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter o orcamento", ex);
            }
        }
    }
}