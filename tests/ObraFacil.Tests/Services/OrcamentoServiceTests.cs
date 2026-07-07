using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;
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
    public async Task CriarAsync_ConflitoDePersistencia_RetentaComProximoNumero()
    {
        var cliente = new Cliente { Id = 1, Nome = "João" };
        _clientes.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(cliente);
        _config.GetAsync(Arg.Any<CancellationToken>())
               .Returns(new Configuracao { ValidadePadraoEmDias = 15 });
        _orcamentos.GerarProximoNumeroAsync(Arg.Any<CancellationToken>())
                   .Returns("2026-0001", "2026-0002");

        var addAttempts = 0;
        _orcamentos.AddAsync(Arg.Any<Orcamento>(), Arg.Any<CancellationToken>())
                   .Returns(_ =>
                   {
                       addAttempts++;
                       return addAttempts == 1
                           ? Task.FromException<Orcamento>(new DbUpdateException("unique"))
                           : Task.FromResult(new Orcamento { Id = 2, Numero = "2026-0002" });
                   });
        _orcamentos.GetComItensAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
                   .Returns(new Orcamento
                   {
                       Id = 2,
                       Numero = "2026-0002",
                       ClienteId = 1,
                       Cliente = cliente,
                       Status = StatusOrcamento.Rascunho,
                       DataEmissao = DateTime.Today,
                       Itens = []
                   });

        var result = await _sut.CriarAsync(BuildInputDto(clienteId: 1));

        Assert.Equal("2026-0002", result.Numero);
        await _uow.Received(2).BeginAsync(Arg.Any<CancellationToken>());
        await _uow.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DuplicarAsync_OrcamentoExistente_CriaNovoRascunhoComMesmoConteudo()
    {
        var cliente = new Cliente { Id = 1, Nome = "João" };
        var origem = new Orcamento
        {
            Id = 10,
            Numero = "2026-0001",
            ClienteId = 1,
            Cliente = cliente,
            Status = StatusOrcamento.Aprovado,
            DataEmissao = DateTime.Today.AddDays(-5),
            DataValidade = DateTime.Today.AddDays(10),
            Desconto = 5m,
            Observacoes = "Observação",
            CondicoesPagamento = "Pix",
            Itens =
            [
                new ItemOrcamento
                {
                    ItemCatalogoId = 3,
                    DescricaoSnapshot = "Cimento",
                    UnidadeSnapshot = UnidadeMedida.Unidade,
                    PrecoUnitarioSnapshot = 40m,
                    CategoriaSnapshot = "Material",
                    Quantidade = 2m,
                    DescontoItem = 1m
                }
            ]
        };
        var duplicado = new Orcamento
        {
            Id = 11,
            Numero = "2026-0002",
            ClienteId = 1,
            Cliente = cliente,
            Status = StatusOrcamento.Rascunho,
            DataEmissao = DateTime.Today,
            DataValidade = origem.DataValidade,
            Desconto = origem.Desconto,
            Observacoes = origem.Observacoes,
            CondicoesPagamento = origem.CondicoesPagamento,
            Itens = origem.Itens.Select(i => new ItemOrcamento
            {
                ItemCatalogoId = i.ItemCatalogoId,
                DescricaoSnapshot = i.DescricaoSnapshot,
                UnidadeSnapshot = i.UnidadeSnapshot,
                PrecoUnitarioSnapshot = i.PrecoUnitarioSnapshot,
                CategoriaSnapshot = i.CategoriaSnapshot,
                Quantidade = i.Quantidade,
                DescontoItem = i.DescontoItem
            }).ToList()
        };

        _orcamentos.GetComItensAsync(10, Arg.Any<CancellationToken>()).Returns(origem);
        _clientes.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(cliente);
        _config.GetAsync(Arg.Any<CancellationToken>())
               .Returns(new Configuracao { ValidadePadraoEmDias = 15 });
        _orcamentos.GerarProximoNumeroAsync(Arg.Any<CancellationToken>()).Returns("2026-0002");
        _orcamentos.AddAsync(Arg.Any<Orcamento>(), Arg.Any<CancellationToken>())
                   .Returns(x =>
                   {
                       var novo = x.ArgAt<Orcamento>(0);
                       novo.Id = 11;
                       return novo;
                   });
        _orcamentos.GetComItensAsync(11, Arg.Any<CancellationToken>()).Returns(duplicado);

        var result = await _sut.DuplicarAsync(10);

        Assert.Equal("2026-0002", result.Numero);
        Assert.Equal(StatusOrcamento.Rascunho, result.Status);
        Assert.Equal(origem.ClienteId, result.ClienteId);
        Assert.Equal(origem.Desconto, result.Desconto);
        Assert.Single(result.Itens);
        await _orcamentos.Received(1).AddAsync(
            Arg.Is<Orcamento>(o => o.Numero == "2026-0002" && o.Itens.Count == 1),
            Arg.Any<CancellationToken>());
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
    public async Task AtualizarAsync_ClienteInexistente_LancaNotFoundException()
    {
        var orc = new Orcamento { Id = 1, Status = StatusOrcamento.Rascunho, Itens = [] };
        _orcamentos.GetComItensAsync(1, Arg.Any<CancellationToken>()).Returns(orc);
        _clientes.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns((Cliente?)null);

        var dto = BuildInputDto(clienteId: 2);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.AtualizarAsync(1, dto));
    }

    [Fact]
    public async Task AlterarStatusAsync_TransicaoInvalida_LancaExcecao()
    {
        var orc = new Orcamento { Id = 1, Status = StatusOrcamento.Aprovado, Itens = [] };
        _orcamentos.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(orc);

        await Assert.ThrowsAsync<ObraFacilException>(() =>
            _sut.AlterarStatusAsync(1, StatusOrcamento.Enviado));
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
