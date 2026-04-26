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

        modelBuilder.Entity<ClienteEntity>(entity =>
        {
            entity.ToTable("Cliente");
            entity.Property(c => c.Nome).HasMaxLength(200).HasColumnType("varchar(200)");
            entity.Property(c => c.Email).HasMaxLength(200).HasColumnType("varchar(200)");
            entity.Property(c => c.Telefone).HasMaxLength(100).HasColumnType("varchar(100)");
            entity.Property(c => c.Endereco).HasMaxLength(1000).HasColumnType("varchar(1000)");
            entity.Property(c => c.Ativo).HasDefaultValue(false);
            entity.Property(c => c.Excluido).HasDefaultValue(false);
            entity.Property(c => c.CNPJ).HasMaxLength(50).HasColumnType("varchar(50)");
            entity.Property(c => c.CPF).HasMaxLength(50).HasColumnType("varchar(50)");
        });
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
