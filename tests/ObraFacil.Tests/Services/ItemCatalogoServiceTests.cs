using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using ObraFacil.Application.DTOs;
using ObraFacil.Application.Services;
using ObraFacil.Domain.Entities;
using ObraFacil.Domain.Enums;
using ObraFacil.Domain.Exceptions;
using ObraFacil.Domain.Interfaces;

namespace ObraFacil.Tests.Services;

public class ItemCatalogoServiceTests
{
    private readonly IItemCatalogoRepository _repo = Substitute.For<IItemCatalogoRepository>();
    private readonly ItemCatalogoService _sut;

    public ItemCatalogoServiceTests()
    {
        _sut = new ItemCatalogoService(_repo, NullLogger<ItemCatalogoService>.Instance);
    }

    [Fact]
    public async Task CriarAsync_NomeVazio_LancaExcecao()
    {
        var dto = new ItemCatalogoInputDto(TipoItem.Material, "", null, UnidadeMedida.Unidade, 10m, null);
        await Assert.ThrowsAsync<ObraFacilException>(() => _sut.CriarAsync(dto));
    }

    [Fact]
    public async Task CriarAsync_PrecoNegativo_LancaExcecao()
    {
        var dto = new ItemCatalogoInputDto(TipoItem.Material, "Cimento", null, UnidadeMedida.Quilograma, -1m, null);
        await Assert.ThrowsAsync<ObraFacilException>(() => _sut.CriarAsync(dto));
    }

    [Fact]
    public async Task CriarAsync_DadosValidos_ChamaRepositorio()
    {
        var dto = new ItemCatalogoInputDto(TipoItem.Material, "Cimento", null, UnidadeMedida.Quilograma, 5.50m, "Material");
        _repo.AddAsync(Arg.Any<ItemCatalogo>(), Arg.Any<CancellationToken>())
             .Returns(x => x.ArgAt<ItemCatalogo>(0));

        await _sut.CriarAsync(dto);

        await _repo.Received(1).AddAsync(
            Arg.Is<ItemCatalogo>(i => i.Nome == "Cimento" && i.PrecoUnitario == 5.50m),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ObterAsync_IdInexistente_LancaNotFoundException()
    {
        _repo.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((ItemCatalogo?)null);
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.ObterAsync(99));
    }
}
