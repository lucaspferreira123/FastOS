using FastOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FastOS.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261020000000_AddRelatorioPdfOrdem")]
public partial class AddRelatorioPdfOrdem : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.AddColumn<string>(
        name: "RelatorioPdfFileName", table: "OrdemServico", type: "nvarchar(100)", maxLength: 100, nullable: true);

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn(
        name: "RelatorioPdfFileName", table: "OrdemServico");
}
