using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TesteMVC.Dto;
using TesteMVC.IRepository;
using TesteMVC.Models;
using TesteMVC.Repository;

namespace MeuProjeto.Repository
{
    public class OrcamentoRepository
      : BaseRepository<OrcamentoViewModel>, IOrcamentoRepository
    {
        private readonly AppDbContext _context;

        public OrcamentoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<OrcamentoViewModel> AlterarOrcamento(OrcamentoViewModel model)
        {
            try
            {
                // Buscar o orçamento no banco
                var orcamento = await _context.Orcamento
                    .FirstOrDefaultAsync(o => o.idOrdemServico == model.idOrdemServico);

                if (orcamento == null)
                    throw new Exception("Orçamento não encontrado.");

                // Atualizar os campos
                orcamento.idOrdemServico = model.idOrdemServico;
                orcamento.MaoDeObra = model.MaoDeObra;
                orcamento.Materiais = model.Materiais;
                orcamento.Desconto = model.Desconto;
                orcamento.TaxasExtras = model.TaxasExtras;
                orcamento.ValorFinal = model.ValorFinal;
                orcamento.FormaPagamento = model.FormaPagamento;

                // Salvar alterações
                await _context.SaveChangesAsync();

                return orcamento;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Erro ao atualizar o orçamento: {ex.InnerException?.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar o orçamento.", ex);
            }
        }

        public async Task<OrcamentoViewModel> ObterOrcamentoPorOrdemServico(int idOrdemServico)
        {
            try
            {
                var orcamento = await _context.Orcamento
                    .FirstOrDefaultAsync(o => o.idOrdemServico == idOrdemServico);

                return orcamento;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar o orçamento da Ordem de Serviço.", ex);
            }
        }

    }
}
