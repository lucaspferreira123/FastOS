using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastOS.Infrastructure.Migrations
{
    public partial class AdicionarValoresProduto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ValorCusto",
                table: "Produto",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorVenda",
                table: "Produto",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorCusto",
                table: "Produto");

            migrationBuilder.DropColumn(
                name: "ValorVenda",
                table: "Produto");
        }
    }
}
