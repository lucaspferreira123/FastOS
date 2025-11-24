using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TesteMVC.Dto;
using TesteMVC.Models;

namespace MeuProjeto.Repository
{
    public class OrdemRepository
    {
        private readonly AppDbContext _context;

        public OrdemRepository(AppDbContext context)
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
    }
}
