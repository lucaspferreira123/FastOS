using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TesteMVC.Dto;

namespace TesteMVC.Relatorios
{
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
            container.Page(page =>
            {
                page.Margin(30);

                page.Header().PaddingBottom(20).Text("Orçamento").FontSize(24).Bold().AlignCenter();

                page.Content().Column(column =>
                {
                    column.Item().PaddingBottom(10).Text($"Numero da Ordem de Serviço: {Model.OrdemServico.idOrdemServico}");
                    column.Item().PaddingBottom(10).Text($"Cliente: {Model.OrdemServico.ClienteNome}");
                    column.Item().PaddingBottom(10).Text($"Data de Abertura: {Model.OrdemServico.DataAbertura.ToString("f")}");
                    column.Item().PaddingBottom(10).Text($"Previsão de Entrega: {Model.OrdemServico.PrevisaoEntrega.ToString("f")}");

                    column.Item().Element(HeaderCellStyleComponent).PaddingTop(10).Text($"Descrição Serviço");


                    column.Item().PaddingBottom(10).Text($"{Model.OrdemServico.DescricaoServico}");
                   
                    if (Model.Itens.Any() || Model.Itens != null)
                    {
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().ColumnSpan(4).Element(HeaderCellStyleComponent).Text("Itens da Ordem de Serviço");
                                header.Cell().ColumnSpan(1).Element(HeaderCellStyleTabela).Text("Codigo");
                                header.Cell().ColumnSpan(1).Element(HeaderCellStyleTabela).Text("Quantidade");
                                header.Cell().ColumnSpan(1).Element(HeaderCellStyleTabela).Text("Descrição");
                                header.Cell().ColumnSpan(1).Element(HeaderCellStyleTabela).Text("Valor Unitario");
                            });

                            foreach (var item in Model.Itens)
                            {
                                table.Cell().ColumnSpan(1).Text($"{item.idProduto}");
                                table.Cell().ColumnSpan(1).Text($"{item.quantidade}");
                                table.Cell().ColumnSpan(1).Text($"{item.nomeProduto}");
                                table.Cell().ColumnSpan(1).Text($"{item.valorUnitario}");
                               
                            }
                        });
                    } else
                    {
                        column.Item().PaddingBottom(10).Text($"Não existem itens cadastrados nessa Ordem de Serviço");
                    }

                    column.Item().PaddingTop(10).Element(HeaderCellStyleComponent)
      .Text("Resumo do Orçamento");

                    column.Item().Column(col =>
                    {
                        col.Item().PaddingBottom(6)
                            .Text($"Valor da Mão de Obra: R$ {Model.Orcamento.MaoDeObra:F2}");

                        col.Item().PaddingBottom(6)
                            .Text($"Valor dos Materiais: R$ {Model.Orcamento.Materiais:F2}");

                        col.Item().PaddingBottom(6)
                            .Text($"Desconto: {Model.Orcamento.Desconto:F2}%");

                        col.Item().PaddingBottom(6)
                            .Text($"Taxas Extras: R$ {Model.Orcamento.TaxasExtras:F2}");

                        col.Item().PaddingBottom(6)
                            .Text($"Forma de Pagamento: {Model.Orcamento.FormaPagamento}");

                        col.Item().PaddingBottom(6)
                            .Text($"Valor Final do Orçamento: R$ {Model.Orcamento.ValorFinal:F2}").Bold();
                    });


                    column.Item().Element(HeaderCellStyleComponent).PaddingTop(10).Text($"Condições comerciais");

                    column.Item().Text(text =>
                    {
                        text.Span("As condições abaixo fazem parte integrante do orçamento emitido pela UTI do PC Informática. Ao aprovar o serviço, o cliente declara ciência e concordância com todos os termos apresentados.");
                        text.EmptyLine();

                        text.Span(" 1. Validade do Orçamento: Este orçamento é válido por 7 dias corridos após a data de emissão.");
                        text.EmptyLine();

                        text.Span(" 2. Autorização de Serviço: Somente após a confirmação do cliente iniciamos qualquer procedimento técnico.");
                        text.EmptyLine();

                        text.Span(" 3. Prazos de Execução: O prazo informado no orçamento é uma estimativa e poderá variar mediante complexidade ou necessidade de peças.");
                        text.EmptyLine();

                        text.Span(" 4. Peças e Componentes: Caso sejam necessários componentes adicionais, estes serão previamente informados para aprovação.");
                        text.EmptyLine();

                        text.Span(" 5. Garantia: Serviços possuem garantia de 90 dias, exclusivamente sobre o reparo realizado, conforme previsto pelo Código de Defesa do Consumidor.");
                        text.EmptyLine();

                        text.Span(" 6. Cancelamento: Caso o cliente cancele o serviço após o diagnóstico, poderá haver cobrança de taxa de análise conforme tabela vigente.");
                        text.EmptyLine();

                        text.Span(" 7. Responsabilidade sobre Dados: A UTI do PC Informática não se responsabiliza por perda de dados caso o dispositivo chegue sem backup.");
                        text.EmptyLine();

                        text.Span(" 8. Equipamentos Não Retirados: Após 90 dias sem retirada, o equipamento poderá ser destinado à descarte ou venda para cobertura de custos, conforme legislação aplicável.");
                    });
                });

                page.Footer().AlignRight().Text(t =>
                {
                    t.Span("Pagina:");
                    t.CurrentPageNumber();
                    t.Span("/");
                    t.TotalPages();
                });
            });
        }

        static IContainer HeaderCellStyleTabela(IContainer container)
        {
            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
        }

        static IContainer HeaderCellStyleComponent(IContainer container)
        {
            return container.DefaultTextStyle(x => x.Bold()).PaddingVertical(10).BorderColor(Colors.Black).AlignCenter();
        }
    }
}
