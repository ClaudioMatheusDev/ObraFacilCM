using ObraFacil.Application.Interfaces;
using ObraFacil.Domain.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ObraFacil.Infrastructure.Services;

public class PdfService : IPdfService
{
    private readonly IOrcamentoRepository    _orcamentos;
    private readonly IConfiguracaoRepository _config;

    public PdfService(IOrcamentoRepository orcamentos, IConfiguracaoRepository config)
    {
        _orcamentos = orcamentos;
        _config     = config;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GerarOrcamentoPdfAsync(int orcamentoId, CancellationToken ct = default)
    {
        var orc = await _orcamentos.GetComItensAsync(orcamentoId, ct)
            ?? throw new Exception($"Orçamento {orcamentoId} não encontrado.");
        var cfg = await _config.GetAsync(ct);

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(t => t.FontSize(10).FontFamily("Arial"));

                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text(cfg.NomeEmpresa).Bold().FontSize(16);
                            if (cfg.TelefoneEmpresa is not null)
                                c.Item().Text($"Tel: {cfg.TelefoneEmpresa}");
                            if (cfg.EmailEmpresa is not null)
                                c.Item().Text($"E-mail: {cfg.EmailEmpresa}");
                            if (cfg.EnderecoEmpresa is not null)
                                c.Item().Text(cfg.EnderecoEmpresa);
                        });
                        row.ConstantItem(130).AlignRight().Column(c =>
                        {
                            c.Item().Text($"ORÇAMENTO Nº {orc.Numero}").Bold().FontSize(13);
                            c.Item().Text($"Emissão: {orc.DataEmissao:dd/MM/yyyy}");
                            if (orc.DataValidade.HasValue)
                                c.Item().Text($"Válido até: {orc.DataValidade:dd/MM/yyyy}");
                            c.Item().Text($"Status: {orc.Status}");
                        });
                    });
                    col.Item().PaddingVertical(4).LineHorizontal(1);
                    col.Item().Text($"Cliente: {orc.Cliente.Nome}").SemiBold();
                    if (orc.Cliente.Telefone is not null)
                        col.Item().Text($"Telefone: {orc.Cliente.Telefone}");
                    if (orc.Cliente.Endereco is not null)
                        col.Item().Text($"Endereço: {orc.Cliente.Endereco}");
                    col.Item().PaddingTop(8).LineHorizontal(1);
                });

                page.Content().PaddingTop(8).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(5); // Descrição
                        c.RelativeColumn(2); // Unidade
                        c.RelativeColumn(2); // Qtd
                        c.RelativeColumn(3); // Valor Unit.
                        c.RelativeColumn(3); // Subtotal
                    });

                    // Header
                    static IContainer HeaderCell(IContainer c) =>
                        c.DefaultTextStyle(t => t.SemiBold()).Padding(4).Background("#EEEEEE");

                    table.Header(h =>
                    {
                        h.Cell().Element(HeaderCell).Text("Descrição");
                        h.Cell().Element(HeaderCell).AlignCenter().Text("Unidade");
                        h.Cell().Element(HeaderCell).AlignCenter().Text("Qtd");
                        h.Cell().Element(HeaderCell).AlignRight().Text("Valor Unit.");
                        h.Cell().Element(HeaderCell).AlignRight().Text("Subtotal");
                    });

                    // Linhas
                    bool alt = false;
                    foreach (var item in orc.Itens)
                    {
                        string bg = alt ? "#F9F9F9" : Colors.White;
                        alt = !alt;

                        static IContainer Cell(IContainer c, string bg) =>
                            c.Background(bg).Padding(4);

                        table.Cell().Element(c => Cell(c, bg)).Text(item.DescricaoSnapshot);
                        table.Cell().Element(c => Cell(c, bg)).AlignCenter().Text(item.UnidadeSnapshot.ToString());
                        table.Cell().Element(c => Cell(c, bg)).AlignCenter().Text(item.Quantidade.ToString("N2"));
                        table.Cell().Element(c => Cell(c, bg)).AlignRight().Text(item.PrecoUnitarioSnapshot.ToString("C2"));
                        table.Cell().Element(c => Cell(c, bg)).AlignRight().Text(item.Subtotal.ToString("C2"));
                    }
                });

                page.Footer().Column(col =>
                {
                    col.Item().LineHorizontal(1);
                    col.Item().PaddingTop(4).AlignRight().Table(t =>
                    {
                        t.ColumnsDefinition(c => { c.RelativeColumn(); c.ConstantColumn(120); });
                        void Row(string label, string valor, bool bold = false)
                        {
                            var tLabel = t.Cell().AlignRight().Text(label);
                            if (bold) tLabel.Bold();
                            var tValor = t.Cell().AlignRight().Text(valor);
                            if (bold) tValor.Bold();
                        }
                        Row("Subtotal:", orc.Subtotal.ToString("C2"));
                        if (orc.Desconto > 0) Row("Desconto:", $"- {orc.Desconto:C2}");
                        Row("TOTAL:", orc.TotalFinal.ToString("C2"), bold: true);
                    });

                    if (!string.IsNullOrWhiteSpace(orc.CondicoesPagamento))
                        col.Item().PaddingTop(8).Text($"Condições de pagamento: {orc.CondicoesPagamento}");
                    if (!string.IsNullOrWhiteSpace(orc.Observacoes))
                        col.Item().PaddingTop(4).Text($"Obs: {orc.Observacoes}");
                });
            });
        });

        return doc.GeneratePdf();
    }
}
