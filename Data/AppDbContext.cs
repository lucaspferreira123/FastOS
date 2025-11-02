using Microsoft.EntityFrameworkCore;
using TesteMVC.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ClienteViewModel> Cliente { get; set; }
    public DbSet<ProdutoViewModel> Produto { get; set; }
    public DbSet<OrdemServicoViewModel> OrdensServico { get; set; }
    public DbSet<ItemOrdemServicoViewModel> ItensOrdemServico { get; set; }
    public DbSet<UsuarioViewModel> Usuario { get; set; }

}
