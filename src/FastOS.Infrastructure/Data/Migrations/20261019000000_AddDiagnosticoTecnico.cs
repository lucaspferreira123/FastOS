using FastOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FastOS.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261019000000_AddDiagnosticoTecnico")]
public partial class AddDiagnosticoTecnico : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.AddColumn<string>(
        name: "DiagnosticoTecnico", table: "OrdemServico", type: "nvarchar(max)", nullable: false, defaultValue: "");

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn(
        name: "DiagnosticoTecnico", table: "OrdemServico");
}
