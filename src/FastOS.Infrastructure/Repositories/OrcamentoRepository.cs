using FastOS.Domain.Entities;
using FastOS.Domain.Interfaces;
using FastOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastOS.Infrastructure.Repositories;

public class OrcamentoRepository : BaseRepository<OrcamentoEntity>, IOrcamentoRepository
{
    private readonly AppDbContext _context;

    public OrcamentoRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<OrcamentoEntity> AlterarOrcamento(OrcamentoEntity model)
    {
        try
        {
            var orcamento = await _context.Orcamento.FirstOrDefaultAsync(o => o.idOrdemServico == model.idOrdemServico);
            if (orcamento == null)
                throw new Exception("Orçamento não encontrado.");

            orcamento.idOrdemServico = model.idOrdemServico;
            orcamento.MaoDeObra = model.MaoDeObra;
            orcamento.Materiais = model.Materiais;
            orcamento.Desconto = model.Desconto;
            orcamento.TaxasExtras = model.TaxasExtras;
            orcamento.ValorFinal = model.ValorFinal;
            orcamento.FormaPagamento = model.FormaPagamento;

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

    public async Task<OrcamentoEntity?> ObterOrcamentoPorOrdemServico(int idOrdemServico)
    {
        try
        {
            return await _context.Orcamento.FirstOrDefaultAsync(o => o.idOrdemServico == idOrdemServico);
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao buscar o orçamento da Ordem de Serviço.", ex);
        }
    }
}
