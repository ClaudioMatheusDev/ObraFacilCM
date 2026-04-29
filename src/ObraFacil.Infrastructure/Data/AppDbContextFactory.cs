using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ObraFacil.Infrastructure.Data;

/// <summary>
/// Usado apenas pelas ferramentas do EF Core (dotnet ef migrations).
/// Em produção o DbContext é criado via DI com o caminho real do banco.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=design-time.db")
            .Options;
        return new AppDbContext(options);
    }
}
