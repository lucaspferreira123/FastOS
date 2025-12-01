using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TesteMVC.Dto;
using TesteMVC.IRepository;
using TesteMVC.Models;
using TesteMVC.Repository;

namespace MeuProjeto.Repository
{
    public class OrdemServicoRepository
      : BaseRepository<OrdemServicoViewModel>, IOrdemServicoRepository
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
                var ordens = await _context.OrdensServico
        .Select(o => new OrdemServicoDto
        {
            idOrdemServico = o.idOrdemServico,
            ClienteNome = _context.Cliente
                                 .Where(c => c.idCliente == o.idCliente)
                                 .Select(c => c.Nome)
                                 .FirstOrDefault(),

            StatusDescricao = _context.Status
                                      .Where(s => s.idStatus == o.idStatus)
                                      .Select(s => s.Descricao)
                                      .FirstOrDefault(),

            Pago = o.Pago,
            DescricaoServico = o.DescricaoServico,
            DataAbertura = o.DataAbertura,
            PrevisaoEntrega = o.PrevisaoEntrega
        })
        .ToListAsync();

                return ordens;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter os ordens", ex);
            }
        }
        public async Task<OrdemServicoDto> ObterOrdemDto(int idOrdem)
        {
            try
            {
                var ordem = await _context.OrdensServico
            .Where(o => o.idOrdemServico == idOrdem)
            .Select(o => new OrdemServicoDto
            {
                idOrdemServico = o.idOrdemServico,

                idCliente = o.idCliente,
                idStatus = o.idStatus,

                ClienteNome = _context.Cliente
                                      .Where(c => c.idCliente == o.idCliente)
                                      .Select(c => c.Nome)
                                      .FirstOrDefault(),

                StatusDescricao = _context.Status
                                          .Where(s => s.idStatus == o.idStatus)
                                          .Select(s => s.Descricao)
                                          .FirstOrDefault(),

                Pago = o.Pago,
                DescricaoServico = o.DescricaoServico,
                DataAbertura = o.DataAbertura,
                PrevisaoEntrega = o.PrevisaoEntrega
            })
            .FirstOrDefaultAsync();

                return ordem;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter os ordens", ex);
            }
        }

        public async Task<OrdemServicoViewModel> AlterarOrdemServico(OrdemServicoViewModel model)
        {
            try
            {
                // Buscar a ordem no banco
                var ordem = await _context.OrdensServico
                    .FirstOrDefaultAsync(o => o.idOrdemServico == model.idOrdemServico);

                if (ordem == null)
                    throw new Exception("Ordem de Serviço não encontrada.");

                // Atualizar os campos
                ordem.idCliente = model.idCliente;
                ordem.Pago = model.Pago;
                ordem.idStatus = model.idStatus;
                ordem.DescricaoServico = model.DescricaoServico;
                ordem.DataAbertura = model.DataAbertura;
                ordem.PrevisaoEntrega = model.PrevisaoEntrega;

                // Salvar alterações
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
}
