using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QRCoder;
using FastOS.Domain.ValueObjects;

namespace FastOS.Application.Reports
{
    public class ReciboCobrancaDocument : IDocument
    {
        // ── Dados do beneficiário PIX ────────────────────────────────────
        private const string ChavePix        = "luisgomesfelipe7@gmail.com";
        private const string NomeBeneficiario = "Luis Gomes";
        private const string Cidade          = "Americana";

        public RelatorioOrcamentoDto Model { get; set; }

        public ReciboCobrancaDocument(RelatorioOrcamentoDto model)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            Model = model;
        }

        public void Compose(IDocumentContainer container)
        {
            // Gera QR code PIX
            var txId    = $"OS{Model.OrdemServico.idOrdemServico:D8}";
            var payload = PixPayloadHelper.Gerar(ChavePix, NomeBeneficiario, Cidade, Model.Orcamento.ValorFinal, txId);
            var qrBytes = GerarQrCodeBytes(payload);

            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(35);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Content().Column(col =>
                {
                    // ── Cabeçalho ─────────────────────────────────────────
                    col.Item().Border(1).BorderColor(Colors.Grey.Darken2).Padding(12).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("UTI do PC Informatica").FontSize(16).Bold();
                            c.Item().Text("Assistencia Tecnica em Informatica").FontSize(9).FontColor(Colors.Grey.Darken1);
                            c.Item().PaddingTop(4).Text("Rua Americana, 100 - Americana/SP  |  (19) 99900-0000");
                            c.Item().Text("contato@utipc.com.br  |  CNPJ: 00.000.000/0001-00");
                        });

                        row.ConstantItem(130).AlignRight().Column(c =>
                        {
                            c.Item().Background(Colors.Red.Darken2).Padding(8).AlignCenter()
                                .Text("RECIBO DE COBRANCA").FontColor(Colors.White).Bold().FontSize(11);
                            c.Item().PaddingTop(4).AlignRight()
                                .Text($"No OS: {Model.OrdemServico.idOrdemServico}").Bold();
                            c.Item().AlignRight()
                                .Text($"Emissao: {DateTime.Now:dd/MM/yyyy}").FontSize(9);
                        });
                    });

                    col.Item().Height(8);

                    // ── Dados do cliente ──────────────────────────────────
                    col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(10).Column(c =>
                    {
                        c.Item().Text("DADOS DO CLIENTE").Bold().FontSize(9).FontColor(Colors.Grey.Darken2);
                        c.Item().PaddingTop(4).Text($"Nome: {Model.OrdemServico.ClienteNome}").Bold();
                        c.Item().PaddingTop(2).Text($"Data de Abertura: {Model.OrdemServico.DataAbertura:dd/MM/yyyy}");
                        c.Item().Text($"Previsao de Entrega: {Model.OrdemServico.PrevisaoEntrega:dd/MM/yyyy}");
                    });

                    col.Item().Height(8);

                    // ── Descrição do serviço ──────────────────────────────
                    col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(10).Column(c =>
                    {
                        c.Item().Text("DESCRICAO DO SERVICO").Bold().FontSize(9).FontColor(Colors.Grey.Darken2);
                        c.Item().PaddingTop(4).Text(Model.OrdemServico.DescricaoServico);
                    });

                    col.Item().Height(8);

                    // ── Itens utilizados ──────────────────────────────────
                    if (Model.Itens != null && Model.Itens.Any())
                    {
                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(10).Column(c =>
                        {
                            c.Item().Text("PECAS E MATERIAIS UTILIZADOS").Bold().FontSize(9).FontColor(Colors.Grey.Darken2);

                            c.Item().PaddingTop(6).Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.ConstantColumn(50);
                                    cols.RelativeColumn(3);
                                    cols.ConstantColumn(60);
                                    cols.ConstantColumn(80);
                                    cols.ConstantColumn(80);
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Element(CellHeader).Text("Codigo");
                                    h.Cell().Element(CellHeader).Text("Descricao");
                                    h.Cell().Element(CellHeader).AlignCenter().Text("Qtd");
                                    h.Cell().Element(CellHeader).AlignRight().Text("Vl. Unit.");
                                    h.Cell().Element(CellHeader).AlignRight().Text("Subtotal");
                                });

                                foreach (var item in Model.Itens)
                                {
                                    var subtotal = decimal.TryParse(item.quantidade, out var qtd)
                                        ? qtd * item.valorUnitario : 0;

                                    table.Cell().Element(CellBody).Text($"{item.idProduto}");
                                    table.Cell().Element(CellBody).Text(item.nomeProduto);
                                    table.Cell().Element(CellBody).AlignCenter().Text($"{item.quantidade}");
                                    table.Cell().Element(CellBody).AlignRight().Text($"R$ {item.valorUnitario:F2}");
                                    table.Cell().Element(CellBody).AlignRight().Text($"R$ {subtotal:F2}");
                                }
                            });
                        });

                        col.Item().Height(8);
                    }

                    // ── Resumo financeiro + QR Code PIX ──────────────────
                    col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(10).Row(row =>
                    {
                        // Resumo à esquerda
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("RESUMO FINANCEIRO").Bold().FontSize(9).FontColor(Colors.Grey.Darken2);
                            c.Item().PaddingTop(6).Text($"Mao de Obra:        R$ {Model.Orcamento.MaoDeObra:F2}");
                            c.Item().PaddingTop(2).Text($"Materiais:          R$ {Model.Orcamento.Materiais:F2}");
                            c.Item().PaddingTop(2).Text($"Desconto:           {Model.Orcamento.Desconto:F2}%");
                            c.Item().PaddingTop(2).Text($"Taxas Extras:       R$ {Model.Orcamento.TaxasExtras:F2}");
                            c.Item().PaddingTop(2).Text($"Forma de Pagamento: {Model.Orcamento.FormaPagamento}");

                            c.Item().PaddingTop(10).Border(1).BorderColor(Colors.Red.Darken2)
                                .Background(Colors.Red.Lighten4).Padding(8).Column(inner =>
                                {
                                    inner.Item().AlignCenter().Text("VALOR TOTAL").Bold().FontSize(9).FontColor(Colors.Red.Darken2);
                                    inner.Item().PaddingTop(4).AlignCenter()
                                        .Text($"R$ {Model.Orcamento.ValorFinal:F2}")
                                        .Bold().FontSize(18).FontColor(Colors.Red.Darken2);
                                });
                        });

                        // QR Code PIX à direita
                        row.ConstantItem(160).AlignCenter().Column(c =>
                        {
                            c.Item().AlignCenter().Text("PAGUE VIA PIX").Bold().FontSize(9).FontColor(Colors.Grey.Darken2);
                            c.Item().PaddingTop(6).AlignCenter()
                                .Image(qrBytes).FitWidth();
                            c.Item().PaddingTop(4).AlignCenter()
                                .Text($"R$ {Model.Orcamento.ValorFinal:F2}").Bold().FontSize(11).FontColor(Colors.Red.Darken2);
                            c.Item().PaddingTop(2).AlignCenter()
                                .Text(NomeBeneficiario).FontSize(8).FontColor(Colors.Grey.Darken1);
                            c.Item().AlignCenter()
                                .Text(ChavePix).FontSize(7).FontColor(Colors.Grey.Darken1);
                        });
                    });

                    col.Item().Height(8);

                    // ── Instruções de pagamento ───────────────────────────
                    col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(10).Column(c =>
                    {
                        c.Item().Text("INSTRUCOES DE PAGAMENTO").Bold().FontSize(9).FontColor(Colors.Grey.Darken2);
                        c.Item().PaddingTop(4).Text(text =>
                        {
                            text.Span("- Pagamento deve ser realizado no ato da retirada do equipamento.");
                            text.EmptyLine();
                            text.Span("- Aceitamos: Dinheiro, PIX, Cartao de Debito e Credito.");
                            text.EmptyLine();
                            text.Span($"- PIX: {ChavePix} - {NomeBeneficiario}.");
                            text.EmptyLine();
                            text.Span("- Equipamentos nao retirados em 90 dias poderao ser descartados conforme legislacao vigente.");
                        });
                    });

                    col.Item().Height(8);

                    // ── Assinaturas ───────────────────────────────────────
                    col.Item().PaddingTop(20).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().BorderBottom(1).BorderColor(Colors.Black).Height(30);
                            c.Item().PaddingTop(4).AlignCenter().Text("Responsavel Tecnico").FontSize(9);
                            c.Item().AlignCenter().Text("UTI do PC Informatica").FontSize(9).FontColor(Colors.Grey.Darken1);
                        });

                        row.ConstantItem(40);

                        row.RelativeItem().Column(c =>
                        {
                            c.Item().BorderBottom(1).BorderColor(Colors.Black).Height(30);
                            c.Item().PaddingTop(4).AlignCenter().Text("Assinatura do Cliente").FontSize(9);
                            c.Item().AlignCenter().Text(Model.OrdemServico.ClienteNome).FontSize(9).FontColor(Colors.Grey.Darken1);
                        });
                    });
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Pagina ").FontSize(8).FontColor(Colors.Grey.Darken1);
                    t.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Darken1);
                    t.Span(" de ").FontSize(8).FontColor(Colors.Grey.Darken1);
                    t.TotalPages().FontSize(8).FontColor(Colors.Grey.Darken1);
                });
            });
        }

        // ── Gera QR code como PNG bytes ───────────────────────────────────
        private static byte[] GerarQrCodeBytes(string payload)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData      = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.M);
            using var qrCode      = new PngByteQRCode(qrData);
            return qrCode.GetGraphic(6);
        }

        static IContainer CellHeader(IContainer c) =>
            c.DefaultTextStyle(x => x.Bold().FontSize(9))
             .PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1)
             .Background(Colors.Grey.Lighten3);

        static IContainer CellBody(IContainer c) =>
            c.DefaultTextStyle(x => x.FontSize(9))
             .PaddingVertical(3).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
    }
}
