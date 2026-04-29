using Microsoft.EntityFrameworkCore;
using ObraFacil.Domain.Entities;
using ObraFacil.Infrastructure.Data;
using ObraFacil.Infrastructure.Repositories;

namespace ObraFacil.Tests.Repositories;

public class ClienteRepositoryTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly ClienteRepository _sut;

    public ClienteRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db  = new AppDbContext(options);
        _sut = new ClienteRepository(_db);
    }

    [Fact]
    public async Task BuscarAsync_PorNome_RetornaClientesCorretos()
    {
        _db.Clientes.AddRange(
            new Cliente { Nome = "João Silva",  Telefone = "11999990001" },
            new Cliente { Nome = "Maria Santos", Telefone = "11999990002" });
        await _db.SaveChangesAsync();

        var result = await _sut.BuscarAsync("João");

        Assert.Single(result);
        Assert.Equal("João Silva", result[0].Nome);
    }

    [Fact]
    public async Task BuscarAsync_PorTelefone_RetornaClienteCorreto()
    {
        _db.Clientes.AddRange(
            new Cliente { Nome = "João Silva",   Telefone = "11999990001" },
            new Cliente { Nome = "Maria Santos", Telefone = "11999990002" });
        await _db.SaveChangesAsync();

        var result = await _sut.BuscarAsync("0002");

        Assert.Single(result);
        Assert.Equal("Maria Santos", result[0].Nome);
    }

    [Fact]
    public async Task BuscarAsync_TermoInexistente_RetornaListaVazia()
    {
        _db.Clientes.Add(new Cliente { Nome = "João" });
        await _db.SaveChangesAsync();

        var result = await _sut.BuscarAsync("zzz");

        Assert.Empty(result);
    }

    public void Dispose() => _db.Dispose();
}
