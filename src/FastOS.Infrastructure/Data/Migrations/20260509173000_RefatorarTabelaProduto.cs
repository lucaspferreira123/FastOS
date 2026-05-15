using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastOS.Infrastructure.Migrations
{
    public partial class RefatorarTabelaProduto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "idProduto",
                table: "Produto",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "QuantidadeTotal",
                table: "Produto",
                newName: "Quantidade");

            migrationBuilder.DropColumn(
                name: "Marca",
                table: "Produto");

            migrationBuilder.DropColumn(
                name: "NomeProduto",
                table: "Produto");

            migrationBuilder.DropColumn(
                name: "PrecoUnitario",
                table: "Produto");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantidade",
                table: "Produto",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Produto",
                type: "varchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Produto",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Excluido",
                table: "Produto",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "QuantidadeMinimaEstoque",
                table: "Produto",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Produto");

            migrationBuilder.DropColumn(
                name: "Excluido",
                table: "Produto");

            migrationBuilder.DropColumn(
                name: "QuantidadeMinimaEstoque",
                table: "Produto");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Produto",
                newName: "idProduto");

            migrationBuilder.RenameColumn(
                name: "Quantidade",
                table: "Produto",
                newName: "QuantidadeTotal");

            migrationBuilder.AlterColumn<int>(
                name: "QuantidadeTotal",
                table: "Produto",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Produto",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AddColumn<string>(
                name: "Marca",
                table: "Produto",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeProduto",
                table: "Produto",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoUnitario",
                table: "Produto",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
