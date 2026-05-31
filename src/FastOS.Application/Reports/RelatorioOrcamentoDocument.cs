using FastOS.Domain.ValueObjects;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FastOS.Application.Reports;

public class RelatorioOrcamentoDocument : IDocument
{
    public RelatorioOrcamentoDto Model { get; set; }

    public RelatorioOrcamentoDocument(RelatorioOrcamentoDto model)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        Model = model;
    }

    public void Compose(IDocumentContainer container)
    {
        var itens = Model.Itens ?? [];
        var totalPecas = itens.Sum(item =>
        {
            var quantidade = decimal.TryParse(item.quantidade, out var qtd) ? qtd : 0;
            return quantidade * item.valorUnitario;
        });

        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(28);
            page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));
            page.Background(Colors.White);

            page.Header().Element(ComposeHeader);
            page.Content().Column(column =>
            {
                column.Spacing(12);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Element(c => ComposeInfoCard(c, "ORDEM DE SERVIÇO", new[]
                    {
                        ("Número", Model.OrdemServico.idOrdemServico.ToString()),
                        ("Cliente", Model.OrdemServico.ClienteNome),
                        ("Abertura", Model.OrdemServico.DataAbertura.ToString("dd/MM/yyyy")),
                        ("Previsão", Model.OrdemServico.PrevisaoEntrega.ToString("dd/MM/yyyy"))
                    }));

                    row.ConstantItem(14);

                    row.RelativeItem().Element(c => ComposeResumoCard(c, totalPecas));
                });

                column.Item().Element(c => ComposeDescricaoServico(c));

                if (itens.Any())
                {
                    column.Item().Element(c => ComposeTabelaItens(c, itens, totalPecas));
                }
                else
                {
                    column.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(12)
                        .Text("Não existem itens cadastrados nessa Ordem de Serviço")
                        .FontColor(Colors.Grey.Darken1);
                }

                column.Item().Element(c => ComposeCondicoes(c));
                column.Item().Element(c => ComposeAssinaturas(c));
            });

            page.Footer().PaddingTop(6).AlignCenter().Text(t =>
            {
                t.Span("Página ").FontSize(8).FontColor(Colors.Grey.Darken1);
                t.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Darken1);
                t.Span(" de ").FontSize(8).FontColor(Colors.Grey.Darken1);
                t.TotalPages().FontSize(8).FontColor(Colors.Grey.Darken1);
            });
        });
    }

    private static void ComposeHeader(IContainer container)
    {
        container
            .PaddingBottom(6)
            .BorderBottom(2)
            .BorderColor(Colors.Red.Darken2)
            .Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("ORÇAMENTO").FontSize(22).Bold().FontColor(Colors.Red.Darken2);
                    col.Item().PaddingTop(2).Text("Proposta técnica e financeira").FontSize(10).FontColor(Colors.Grey.Darken1);
                });

                row.ConstantItem(180).AlignRight().Column(col =>
                {
                    col.Item().Background(Colors.Red.Darken2).PaddingVertical(8).PaddingHorizontal(12).AlignCenter()
                        .Text("UTI do PC Informática").FontColor(Colors.White).Bold().FontSize(11);
                    col.Item().PaddingTop(4).AlignRight()
                        .Text("Assistência Técnica em Informática").FontSize(8).FontColor(Colors.Grey.Darken1);
                    col.Item().AlignRight()
                        .Text($"Emitido em {DateTime.Now:dd/MM/yyyy}").FontSize(8).FontColor(Colors.Grey.Darken1);
                });
            });
    }

    private static void ComposeInfoCard(IContainer container, string title, IEnumerable<(string Label, string Value)> lines)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.White).Padding(12).Column(col =>
        {
            col.Item().Text(title).FontSize(9).Bold().FontColor(Colors.Grey.Darken2);
            col.Item().PaddingTop(6);

            foreach (var (label, value) in lines)
            {
                col.Item().PaddingBottom(4).Row(row =>
                {
                    row.ConstantItem(88).Text($"{label}:").FontColor(Colors.Grey.Darken1).Bold();
                    row.RelativeItem().Text(value).FontColor(Colors.Black);
                });
            }
        });
    }

    private void ComposeResumoCard(IContainer container, decimal totalPecas)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten5).Padding(12).Column(col =>
        {
            col.Item().Text("RESUMO FINANCEIRO").FontSize(9).Bold().FontColor(Colors.Grey.Darken2);
            col.Item().PaddingTop(6).Row(row =>
            {
                row.RelativeItem().Text("Mão de obra").FontColor(Colors.Grey.Darken1);
                row.ConstantItem(100).AlignRight().Text($"R$ {Model.Orcamento.MaoDeObra:F2}");
            });
            col.Item().PaddingTop(3).Row(row =>
            {
                row.RelativeItem().Text("Materiais").FontColor(Colors.Grey.Darken1);
                row.ConstantItem(100).AlignRight().Text($"R$ {Model.Orcamento.Materiais:F2}");
            });
            col.Item().PaddingTop(3).Row(row =>
            {
                row.RelativeItem().Text("Subtotal peças").FontColor(Colors.Grey.Darken1);
                row.ConstantItem(100).AlignRight().Text($"R$ {totalPecas:F2}");
            });
            col.Item().PaddingTop(3).Row(row =>
            {
                row.RelativeItem().Text("Desconto").FontColor(Colors.Grey.Darken1);
                row.ConstantItem(100).AlignRight().Text($"{Model.Orcamento.Desconto:F2}%");
            });
            col.Item().PaddingTop(3).Row(row =>
            {
                row.RelativeItem().Text("Taxas extras").FontColor(Colors.Grey.Darken1);
                row.ConstantItem(100).AlignRight().Text($"R$ {Model.Orcamento.TaxasExtras:F2}");
            });

            col.Item().PaddingTop(10).Border(1).BorderColor(Colors.Red.Darken2).Background(Colors.White).Padding(10).Column(inner =>
            {
                inner.Item().AlignCenter().Text("VALOR FINAL").FontSize(9).Bold().FontColor(Colors.Red.Darken2);
                inner.Item().PaddingTop(4).AlignCenter()
                    .Text($"R$ {Model.Orcamento.ValorFinal:F2}")
                    .FontSize(18).Bold().FontColor(Colors.Red.Darken2);
            });

            col.Item().PaddingTop(8).AlignRight().Text($"Pagamento: {Model.Orcamento.FormaPagamento}")
                .FontSize(8).FontColor(Colors.Grey.Darken1);
        });
    }

    private void ComposeDescricaoServico(IContainer container)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(12).Column(col =>
        {
            col.Item().Text("DESCRIÇÃO DO SERVIÇO").FontSize(9).Bold().FontColor(Colors.Grey.Darken2);
            col.Item().PaddingTop(6).Text(Model.OrdemServico.DescricaoServico).FontColor(Colors.Grey.Darken4);
        });
    }

    private static void ComposeTabelaItens(IContainer container, List<ItensOrdemServicoDto> itens, decimal totalPecas)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(12).Column(col =>
        {
            col.Item().Text("PEÇAS E MATERIAIS UTILIZADOS").FontSize(9).Bold().FontColor(Colors.Grey.Darken2);
            col.Item().PaddingTop(8).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(50);
                    columns.RelativeColumn(3);
                    columns.ConstantColumn(50);
                    columns.ConstantColumn(82);
                    columns.ConstantColumn(82);
                });

                table.Header(header =>
                {
                    header.Cell().Element(TableHeaderCell).Text("Código");
                    header.Cell().Element(TableHeaderCell).Text("Descrição");
                    header.Cell().Element(TableHeaderCell).AlignCenter().Text("Qtd");
                    header.Cell().Element(TableHeaderCell).AlignRight().Text("Vlr. Unit.");
                    header.Cell().Element(TableHeaderCell).AlignRight().Text("Total");
                });

                var rowIndex = 0;
                foreach (var item in itens)
                {
                    var quantidade = decimal.TryParse(item.quantidade, out var qtd) ? qtd : 0;
                    var subtotal = quantidade * item.valorUnitario;
                    var rowBackground = rowIndex % 2 == 0 ? Colors.White : Colors.Grey.Lighten5;

                    table.Cell().Element(cell => TableBodyCell(cell, rowBackground)).Text($"{item.idProduto}");
                    table.Cell().Element(cell => TableBodyCell(cell, rowBackground)).Text(item.nomeProduto);
                    table.Cell().Element(cell => TableBodyCell(cell, rowBackground)).AlignCenter().Text($"{item.quantidade}");
                    table.Cell().Element(cell => TableBodyCell(cell, rowBackground)).AlignRight().Text($"R$ {item.valorUnitario:F2}");
                    table.Cell().Element(cell => TableBodyCell(cell, rowBackground)).AlignRight().Text($"R$ {subtotal:F2}");

                    rowIndex++;
                }

                table.Cell().ColumnSpan(4).Element(TableTotalLabelCell).Text("Subtotal das peças");
                table.Cell().Element(TableTotalValueCell).Text($"R$ {totalPecas:F2}");
            });
        });
    }

    private static void ComposeCondicoes(IContainer container)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(12).Column(col =>
        {
            col.Item().Text("CONDIÇÕES COMERCIAIS").FontSize(9).Bold().FontColor(Colors.Grey.Darken2);
            col.Item().PaddingTop(6).Text(text =>
            {
                text.Span("1. Validade do Orçamento: válido por 7 dias corridos após a emissão.").FontColor(Colors.Grey.Darken3);
                text.EmptyLine();
                text.Span("2. Execução: o serviço inicia somente após a aprovação do cliente.").FontColor(Colors.Grey.Darken3);
                text.EmptyLine();
                text.Span("3. Prazos: são estimativas e podem variar conforme complexidade ou necessidade de peças.").FontColor(Colors.Grey.Darken3);
                text.EmptyLine();
                text.Span("4. Garantia: 90 dias exclusivamente sobre o reparo realizado.").FontColor(Colors.Grey.Darken3);
                text.EmptyLine();
                text.Span("5. Cancelamento: após diagnóstico, pode haver cobrança de taxa de análise.").FontColor(Colors.Grey.Darken3);
            });
        });
    }

    private void ComposeAssinaturas(IContainer container)
    {
        container.PaddingTop(8).Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().BorderBottom(1).BorderColor(Colors.Black).Height(28);
                col.Item().PaddingTop(4).AlignCenter().Text("Responsável Técnico").FontSize(9).FontColor(Colors.Grey.Darken2);
                col.Item().AlignCenter().Text("UTI do PC Informática").FontSize(8).FontColor(Colors.Grey.Darken1);
            });

            row.ConstantItem(36);

            row.RelativeItem().Column(col =>
            {
                col.Item().BorderBottom(1).BorderColor(Colors.Black).Height(28);
                col.Item().PaddingTop(4).AlignCenter().Text("Assinatura do Cliente").FontSize(9).FontColor(Colors.Grey.Darken2);
                col.Item().AlignCenter().Text(Model.OrdemServico.ClienteNome).FontSize(8).FontColor(Colors.Grey.Darken1);
            });
        });
    }

    private static IContainer TableHeaderCell(IContainer container)
    {
        return container
            .Background(Colors.Red.Darken2)
            .PaddingVertical(6)
            .PaddingHorizontal(6)
            .DefaultTextStyle(x => x.Bold().FontColor(Colors.White))
            .BorderBottom(1)
            .BorderColor(Colors.White);
    }

    private static IContainer TableBodyCell(IContainer container, string background)
    {
        return container
            .Background(background)
            .PaddingVertical(5)
            .PaddingHorizontal(6)
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2);
    }

    private static IContainer TableTotalLabelCell(IContainer container)
    {
        return container
            .Background(Colors.Grey.Lighten4)
            .PaddingVertical(6)
            .PaddingHorizontal(6)
            .AlignRight()
            .DefaultTextStyle(x => x.SemiBold());
    }

    private static IContainer TableTotalValueCell(IContainer container)
    {
        return container
            .Background(Colors.Grey.Lighten4)
            .PaddingVertical(6)
            .PaddingHorizontal(6)
            .AlignRight()
            .DefaultTextStyle(x => x.Bold());
    }
}
