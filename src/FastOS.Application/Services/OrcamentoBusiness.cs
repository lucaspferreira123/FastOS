using FastOS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using FastOS.Domain.ValueObjects;
using FastOS.Domain.Interfaces;
using FastOS.Domain.Entities;

namespace FastOS.Application.Services
{
    public class OrcamentoBusiness
    {
        private readonly IOrcamentoRepository _repository;

        public OrcamentoBusiness(IOrcamentoRepository repository)
        {
            _repository = repository;
        }

        public async Task<OrcamentoEntity> AlterarOrcamento(OrcamentoEntity orcamento)
        {
            try
            {

                var orcamentoAntigo = ObterOrcamento(orcamento.idOrdemServico).Result;

                var orcamentoNovo = new OrcamentoEntity{};

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

        public async Task<OrcamentoEntity> ObterOrcamento(int idOrdem)
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
