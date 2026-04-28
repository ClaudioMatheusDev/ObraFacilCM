using Microsoft.EntityFrameworkCore;
using ObraFacil.Domain.Entities;

namespace ObraFacil.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cliente>       Clientes       => Set<Cliente>();
    public DbSet<ItemCatalogo>  ItensCatalogo  => Set<ItemCatalogo>();
    public DbSet<Orcamento>     Orcamentos     => Set<Orcamento>();
    public DbSet<ItemOrcamento> ItensOrcamento => Set<ItemOrcamento>();
    public DbSet<Configuracao>  Configuracoes  => Set<Configuracao>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Cliente>(e => {
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).IsRequired().HasMaxLength(200);
            e.Property(x => x.Email).HasMaxLength(200);
            e.Property(x => x.Telefone).HasMaxLength(30);
            e.Property(x => x.Documento).HasMaxLength(20);
        });

        mb.Entity<ItemCatalogo>(e => {
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).IsRequired().HasMaxLength(200);
            e.Property(x => x.PrecoUnitario).HasColumnType("decimal(18,4)");
        });

        mb.Entity<Orcamento>(e => {
            e.HasKey(x => x.Id);
            e.Property(x => x.Numero).IsRequired().HasMaxLength(20);
            e.HasIndex(x => x.Numero).IsUnique();
            e.Property(x => x.Desconto).HasColumnType("decimal(18,2)");
            e.Ignore(x => x.Subtotal);
            e.Ignore(x => x.TotalFinal);
            e.HasOne(x => x.Cliente).WithMany(c => c.Orcamentos)
             .HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<ItemOrcamento>(e => {
            e.HasKey(x => x.Id);
            e.Property(x => x.DescricaoSnapshot).IsRequired().HasMaxLength(500);
            e.Property(x => x.PrecoUnitarioSnapshot).HasColumnType("decimal(18,4)");
            e.Property(x => x.Quantidade).HasColumnType("decimal(18,4)");
            e.Property(x => x.DescontoItem).HasColumnType("decimal(18,2)");
            e.Ignore(x => x.Subtotal);
            e.HasOne(x => x.Orcamento).WithMany(o => o.Itens)
             .HasForeignKey(x => x.OrcamentoId).OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<Configuracao>(e => {
            e.HasKey(x => x.Id);
            e.Property(x => x.NomeEmpresa).IsRequired().HasMaxLength(200);
        });
    }
}
