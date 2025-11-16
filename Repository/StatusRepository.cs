using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TesteMVC.Models;

namespace MeuProjeto.Repository
{
    public class StatusRepository
    {
        private readonly AppDbContext _context;

        public StatusRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StatusViewModel>> ObterTodosStatus()
        {
            try
            {
                return await _context.Status.ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter os usuarios", ex);
            }
        }
    }
}
