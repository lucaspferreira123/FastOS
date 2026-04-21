using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TesteMVC.Migrations
{
    /// <inheritdoc />
    public partial class tabelaOrcamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orcamento",
                columns: table => new
                {
                    idOrcamento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idOrdemServico = table.Column<int>(type: "int", nullable: false),
                    MaoDeObra = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Materiais = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Desconto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxasExtras = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorFinal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FormaPagamento = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orcamento", x => x.idOrcamento);
                    table.ForeignKey(
                        name: "FK_Orcamento_OrdemServico_idOrdemServico",
                        column: x => x.idOrdemServico,
                        principalTable: "OrdemServico",
                        principalColumn: "idOrdemServico",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orcamento_idOrdemServico",
                table: "Orcamento",
                column: "idOrdemServico",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orcamento");
        }
    }
}
