using Microsoft.EntityFrameworkCore;
using ObraFacil.Domain.Entities;
using ObraFacil.Infrastructure.Data;
using ObraFacil.Infrastructure.Repositories;

namespace ObraFacil.Tests.Repositories;

public class OrcamentoRepositoryTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly OrcamentoRepository _sut;

    public OrcamentoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db  = new AppDbContext(options);
        _sut = new OrcamentoRepository(_db);
    }

    [Fact]
    public async Task FiltrarAsync_SemFiltros_RetornaTodos()
    {
        var cliente = new Cliente { Nome = "Teste" };
        _db.Clientes.Add(cliente);
        await _db.SaveChangesAsync();

        _db.Orcamentos.AddRange(
            new Orcamento { Numero = "2026-0001", ClienteId = cliente.Id, DataEmissao = DateTime.Today },
            new Orcamento { Numero = "2026-0002", ClienteId = cliente.Id, DataEmissao = DateTime.Today });
        await _db.SaveChangesAsync();

        var result = await _sut.FiltrarAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task FiltrarAsync_FiltroPorNumero_RetornaCorreto()
    {
        var cliente = new Cliente { Nome = "Teste" };
        _db.Clientes.Add(cliente);
        await _db.SaveChangesAsync();

        _db.Orcamentos.AddRange(
            new Orcamento { Numero = "2026-0001", ClienteId = cliente.Id, DataEmissao = DateTime.Today },
            new Orcamento { Numero = "2026-0002", ClienteId = cliente.Id, DataEmissao = DateTime.Today });
        await _db.SaveChangesAsync();

        var result = await _sut.FiltrarAsync(termo: "0001");

        Assert.Single(result);
        Assert.Equal("2026-0001", result[0].Numero);
    }

    [Fact]
    public async Task GerarProximoNumeroAsync_PrimeiroNumero_Retorna0001()
    {
        _db.Configuracoes.Add(new Configuracao { ProximoNumeroOrcamento = 1 });
        await _db.SaveChangesAsync();

        var numero = await _sut.GerarProximoNumeroAsync();

        Assert.Equal($"{DateTime.Today.Year}-0001", numero);
    }

    [Fact]
    public async Task GerarProximoNumeroAsync_IncrementaConfiguracaoCadaChamada()
    {
        _db.Configuracoes.Add(new Configuracao { ProximoNumeroOrcamento = 1 });
        await _db.SaveChangesAsync();

        var n1 = await _sut.GerarProximoNumeroAsync();
        var n2 = await _sut.GerarProximoNumeroAsync();

        Assert.NotEqual(n1, n2);
        Assert.Equal($"{DateTime.Today.Year}-0001", n1);
        Assert.Equal($"{DateTime.Today.Year}-0002", n2);
    }

    public void Dispose() => _db.Dispose();
}
