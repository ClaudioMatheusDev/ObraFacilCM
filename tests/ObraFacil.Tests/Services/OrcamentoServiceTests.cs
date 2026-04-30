using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using ObraFacil.Application.DTOs;
using ObraFacil.Application.Interfaces;
using ObraFacil.Application.Services;
using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Enums;
using ObraFacil.Domain.Exceptions;
using ObraFacil.Domain.Interfaces;

namespace ObraFacil.Tests.Services;

public class OrcamentoServiceTests
{
    private readonly IOrcamentoRepository    _orcamentos = Substitute.For<IOrcamentoRepository>();
    private readonly IClienteRepository      _clientes   = Substitute.For<IClienteRepository>();
    private readonly IConfiguracaoRepository _config     = Substitute.For<IConfiguracaoRepository>();
    private readonly IUnitOfWork             _uow        = Substitute.For<IUnitOfWork>();
    private readonly OrcamentoService        _sut;

    public OrcamentoServiceTests()
    {
        _sut = new OrcamentoService(
            _orcamentos, _clientes, _config, _uow,
            NullLogger<OrcamentoService>.Instance);
    }

    [Fact]
    public async Task CriarAsync_ClienteInexistente_LancaNotFoundException()
    {
        _clientes.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Cliente?)null);
        var dto = BuildInputDto(clienteId: 99);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.CriarAsync(dto));
    }

    [Fact]
    public async Task CriarAsync_DescontoMaiorQueSubtotal_LancaExcecao()
    {
        _clientes.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(new Cliente { Id = 1, Nome = "teste" });

        // Subtotal = 100, desconto = 200
        var dto = new OrcamentoInputDto(
            ClienteId: 1,
            DataValidade: null,
            Desconto: 200m,
            Observacoes: null,
            CondicoesPagamento: null,
            Itens: [new ItemOrcamentoInputDto(null, "Item", UnidadeMedida.Unidade, 100m, null, 1m, 0m)]);

        await Assert.ThrowsAsync<ObraFacilException>(() => _sut.CriarAsync(dto));
    }

    [Fact]
    public async Task CriarAsync_DescontoNegativo_LancaExcecao()
    {
        _clientes.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(new Cliente { Id = 1, Nome = "teste" });

        var dto = new OrcamentoInputDto(1, null, -10m, null, null,
            [new ItemOrcamentoInputDto(null, "Item", UnidadeMedida.Unidade, 50m, null, 1m, 0m)]);

        await Assert.ThrowsAsync<ObraFacilException>(() => _sut.CriarAsync(dto));
    }

    [Fact]
    public async Task CriarAsync_Valido_ChamaRepositorioERetornaDto()
    {
        var cliente = new Cliente { Id = 1, Nome = "João" };
        _clientes.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(cliente);
        _config.GetAsync(Arg.Any<CancellationToken>())
               .Returns(new Configuracao { ValidadePadraoEmDias = 15 });
        _orcamentos.GerarProximoNumeroAsync(Arg.Any<CancellationToken>()).Returns("2026-0001");
        _orcamentos.AddAsync(Arg.Any<Orcamento>(), Arg.Any<CancellationToken>())
                   .Returns(x => x.ArgAt<Orcamento>(0));
        _orcamentos.GetComItensAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
                   .Returns(x => new Orcamento
                   {
                       Id = 1, Numero = "2026-0001", ClienteId = 1,
                       Cliente = cliente, Status = StatusOrcamento.Rascunho,
                       DataEmissao = DateTime.Today, Itens = []
                   });

        var dto = BuildInputDto(clienteId: 1);
        var result = await _sut.CriarAsync(dto);

        Assert.Equal("2026-0001", result.Numero);
        await _orcamentos.Received(1).AddAsync(Arg.Any<Orcamento>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AtualizarAsync_StatusDiferenteDeRascunho_LancaExcecao()
    {
        var orc = new Orcamento { Id = 1, Status = StatusOrcamento.Enviado, Itens = [] };
        _orcamentos.GetComItensAsync(1, Arg.Any<CancellationToken>()).Returns(orc);

        await Assert.ThrowsAsync<ObraFacilException>(() =>
            _sut.AtualizarAsync(1, BuildInputDto(clienteId: 1)));
    }

    [Fact]
    public async Task AlterarStatusAsync_IdInexistente_LancaNotFoundException()
    {
        _orcamentos.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Orcamento?)null);
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.AlterarStatusAsync(99, StatusOrcamento.Aprovado));
    }

    private static OrcamentoInputDto BuildInputDto(int clienteId) =>
        new(clienteId, null, 0m, null, null,
            [new ItemOrcamentoInputDto(null, "Item Teste", UnidadeMedida.Unidade, 10m, null, 1m, 0m)]);
}
