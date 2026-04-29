using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using ObraFacil.Application.DTOs;
using ObraFacil.Application.Services;
using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Exceptions;
using ObraFacil.Domain.Interfaces;

namespace ObraFacil.Tests.Services;

public class ClienteServiceTests
{
    private readonly IClienteRepository _repo = Substitute.For<IClienteRepository>();
    private readonly ClienteService _sut;

    public ClienteServiceTests()
    {
        _sut = new ClienteService(_repo, NullLogger<ClienteService>.Instance);
    }

    [Fact]
    public async Task CriarAsync_NomeVazio_LancaExcecao()
    {
        var dto = new ClienteInputDto("", null, null, null, null, null);
        await Assert.ThrowsAsync<ObraFacilException>(() => _sut.CriarAsync(dto));
    }

    [Theory]
    [InlineData("invalido")]
    [InlineData("@dominio.com")]
    [InlineData("nome@")]
    [InlineData("nome@s")]
    public async Task CriarAsync_EmailInvalido_LancaExcecao(string email)
    {
        var dto = new ClienteInputDto("João", null, email, null, null, null);
        await Assert.ThrowsAsync<ObraFacilException>(() => _sut.CriarAsync(dto));
    }

    [Theory]
    [InlineData("123")]       // menos de 8 dígitos
    [InlineData("1234567890123456")]  // mais de 15 dígitos
    public async Task CriarAsync_TelefoneInvalido_LancaExcecao(string telefone)
    {
        var dto = new ClienteInputDto("João", telefone, null, null, null, null);
        await Assert.ThrowsAsync<ObraFacilException>(() => _sut.CriarAsync(dto));
    }

    [Fact]
    public async Task CriarAsync_DadosValidos_ChamaRepositorio()
    {
        var dto = new ClienteInputDto("Maria Silva", "(11) 98765-4321", "maria@email.com", null, null, null);
        _repo.AddAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>())
             .Returns(x => x.ArgAt<Cliente>(0));

        await _sut.CriarAsync(dto);

        await _repo.Received(1).AddAsync(
            Arg.Is<Cliente>(c => c.Nome == "Maria Silva"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ObterAsync_IdInexistente_LancaNotFoundException()
    {
        _repo.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Cliente?)null);
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.ObterAsync(99));
    }

    [Fact]
    public async Task AtualizarAsync_EmailInvalido_LancaExcecao()
    {
        var cliente = new Cliente { Id = 1, Nome = "Teste" };
        _repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(cliente);

        var dto = new ClienteInputDto("Teste", null, "email-invalido", null, null, null);
        await Assert.ThrowsAsync<ObraFacilException>(() => _sut.AtualizarAsync(1, dto));
    }
}
