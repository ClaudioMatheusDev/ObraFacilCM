using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Enums;
using ObraFacil.Infrastructure.Data;

namespace ObraFacil.Infrastructure.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (db.Clientes.Any()) return;

        db.Clientes.AddRange(
            new Cliente { Nome = "João da Silva",     Telefone = "(11) 99999-0001", Email = "joao@email.com", Endereco = "Rua das Flores, 100" },
            new Cliente { Nome = "Maria Construções", Telefone = "(11) 98888-0002", Documento = "12.345.678/0001-99", Endereco = "Av. Brasil, 500" }
        );

        db.ItensCatalogo.AddRange(
            new ItemCatalogo { Tipo = TipoItem.Material, Nome = "Cimento CP-II 50kg",    Unidade = UnidadeMedida.Unidade, PrecoUnitario = 42.00m,  Categoria = "Cimento" },
            new ItemCatalogo { Tipo = TipoItem.Material, Nome = "Areia média",            Unidade = UnidadeMedida.MetroC,  PrecoUnitario = 120.00m, Categoria = "Agregados" },
            new ItemCatalogo { Tipo = TipoItem.Material, Nome = "Tijolo cerâmico",        Unidade = UnidadeMedida.Unidade, PrecoUnitario = 1.20m,   Categoria = "Alvenaria" },
            new ItemCatalogo { Tipo = TipoItem.Servico,  Nome = "Mão de obra - pedreiro", Unidade = UnidadeMedida.Hora,    PrecoUnitario = 55.00m,  Categoria = "Mão de Obra" },
            new ItemCatalogo { Tipo = TipoItem.Servico,  Nome = "Mão de obra - servente", Unidade = UnidadeMedida.Hora,    PrecoUnitario = 30.00m,  Categoria = "Mão de Obra" }
        );

        if (!db.Configuracoes.Any())
            db.Configuracoes.Add(new Configuracao { Id = 1, NomeEmpresa = "ObraFácil Construções",
                TelefoneEmpresa = "(11) 99999-0000", EmailEmpresa = "contato@obrafacil.com", ValidadePadraoEmDias = 15 });

        await db.SaveChangesAsync();
    }
}
