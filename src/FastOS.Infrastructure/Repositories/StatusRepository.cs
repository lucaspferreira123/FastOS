using FastOS.Domain.Entities;
using FastOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastOS.Infrastructure.Repositories;

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
            throw new Exception("Erro ao obter os status", ex);
        }
    }
}
