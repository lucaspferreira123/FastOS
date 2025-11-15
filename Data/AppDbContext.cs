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
    public DbSet<StatusViewModel> Status { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrdemServicoViewModel>(entity =>
        {
            entity.ToTable("OrdemServico"); // nome correto no banco

            entity.HasMany(o => o.Itens)
                  .WithOne(i => i.OrdemServico)
                  .HasForeignKey(i => i.idOrdemServico)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ItemOrdemServicoViewModel>(entity =>
        {
            entity.ToTable("ItemOrdemServico");
        });

        modelBuilder.Entity<ClienteViewModel>().ToTable("Cliente");
        modelBuilder.Entity<ProdutoViewModel>().ToTable("Produto");
        modelBuilder.Entity<UsuarioViewModel>().ToTable("Usuario");
        modelBuilder.Entity<StatusViewModel>().ToTable("Status");
    }

}
