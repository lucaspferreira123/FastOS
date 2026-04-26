using FastOS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FastOS.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ClienteEntity> Cliente { get; set; }
    public DbSet<ProdutoEntity> Produto { get; set; }
    public DbSet<OrdemServicoEntity> OrdensServico { get; set; }
    public DbSet<ItemOrdemServicoEntity> ItensOrdemServico { get; set; }
    public DbSet<UsuarioEntity> Usuario { get; set; }
    public DbSet<StatusEntity> Status { get; set; }
    public DbSet<OrcamentoEntity> Orcamento { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrdemServicoEntity>(entity =>
        {
            entity.ToTable("OrdemServico");

            entity.HasMany(o => o.Itens)
                  .WithOne(i => i.OrdemServico)
                  .HasForeignKey(i => i.idOrdemServico)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ItemOrdemServicoEntity>(entity =>
        {
            entity.ToTable("ItemOrdemServico");
        });

        modelBuilder.Entity<OrcamentoEntity>(entity =>
        {
            entity.ToTable("Orcamento");

            entity.HasOne(o => o.OrdemServico)
                  .WithOne(os => os.Orcamento)
                  .HasForeignKey<OrcamentoEntity>(o => o.idOrdemServico)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ClienteEntity>().ToTable("Cliente");
        modelBuilder.Entity<ProdutoEntity>().ToTable("Produto");
        modelBuilder.Entity<UsuarioEntity>(entity =>
        {
            entity.ToTable("Usuario");
            entity.Property(u => u.Ativo).HasDefaultValue(false);
            entity.Property(u => u.Excluido).HasDefaultValue(false);
        });
        modelBuilder.Entity<StatusEntity>().ToTable("Status");
    }
}
