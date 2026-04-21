using FastOS.Domain.Entities;
using FastOS.Domain.Interfaces;
using FastOS.Domain.ValueObjects;
using FastOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastOS.Infrastructure.Repositories;

public class OrdemServicoRepository : BaseRepository<OrdemServicoViewModel>, IOrdemServicoRepository
{
    private readonly AppDbContext _context;

    public OrdemServicoRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<OrdemServicoViewModel> CadastrarOrdem(OrdemServicoViewModel ordem)
    {
        try
        {
            _context.OrdensServico.Add(ordem);
            await _context.SaveChangesAsync();
            return ordem;
        }
        catch (DbUpdateException ex)
        {
            var innerMessage = ex.InnerException?.Message;
            throw new Exception($"Erro ao salvar ordem de serviço: {innerMessage}", ex);
        }
    }

    public async Task<List<OrdemServicoDto>> ObterTodasOrdens()
    {
        try
        {
            return await _context.OrdensServico
                .Select(o => new OrdemServicoDto
                {
                    idOrdemServico = o.idOrdemServico,
                    ClienteNome = _context.Cliente.Where(c => c.idCliente == o.idCliente).Select(c => c.Nome).FirstOrDefault() ?? string.Empty,
                    StatusDescricao = _context.Status.Where(s => s.idStatus == o.idStatus).Select(s => s.Descricao).FirstOrDefault() ?? string.Empty,
                    Pago = o.Pago,
                    DescricaoServico = o.DescricaoServico,
                    DataAbertura = o.DataAbertura,
                    PrevisaoEntrega = o.PrevisaoEntrega
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao obter as ordens", ex);
        }
    }

    public async Task<OrdemServicoDto?> ObterOrdemDto(int idOrdem)
    {
        try
        {
            return await _context.OrdensServico
                .Where(o => o.idOrdemServico == idOrdem)
                .Select(o => new OrdemServicoDto
                {
                    idOrdemServico = o.idOrdemServico,
                    idCliente = o.idCliente,
                    idStatus = o.idStatus,
                    ClienteNome = _context.Cliente.Where(c => c.idCliente == o.idCliente).Select(c => c.Nome).FirstOrDefault() ?? string.Empty,
                    StatusDescricao = _context.Status.Where(s => s.idStatus == o.idStatus).Select(s => s.Descricao).FirstOrDefault() ?? string.Empty,
                    Pago = o.Pago,
                    DescricaoServico = o.DescricaoServico,
                    DataAbertura = o.DataAbertura,
                    PrevisaoEntrega = o.PrevisaoEntrega
                })
                .FirstOrDefaultAsync();
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
            var ordem = await _context.OrdensServico.FirstOrDefaultAsync(o => o.idOrdemServico == model.idOrdemServico);
            if (ordem == null)
                throw new Exception("Ordem de Serviço não encontrada.");

            ordem.idCliente = model.idCliente;
            ordem.Pago = model.Pago;
            ordem.idStatus = model.idStatus;
            ordem.DescricaoServico = model.DescricaoServico;
            ordem.DataAbertura = model.DataAbertura;
            ordem.PrevisaoEntrega = model.PrevisaoEntrega;

            await _context.SaveChangesAsync();
            return ordem;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception($"Erro ao atualizar a Ordem de Serviço: {ex.InnerException?.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao atualizar a Ordem de Serviço", ex);
        }
    }
}
