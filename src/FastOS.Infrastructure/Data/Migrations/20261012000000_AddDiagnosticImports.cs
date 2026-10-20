using FastOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FastOS.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261012000000_AddDiagnosticImports")]
public partial class AddDiagnosticImports : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "DiagnosticImport",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                DiagnosticId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OrdemServicoId = table.Column<int>(type: "int", nullable: false),
                GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                ComputerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Summary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                DiagnosticJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PdfFileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                ImportedByUserId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DiagnosticImport", x => x.Id);
                table.ForeignKey("FK_DiagnosticImport_OrdemServico_OrdemServicoId", x => x.OrdemServicoId, "OrdemServico", "idOrdemServico", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex("IX_DiagnosticImport_DiagnosticId", "DiagnosticImport", "DiagnosticId", unique: true);
        migrationBuilder.CreateIndex("IX_DiagnosticImport_OrdemServicoId", "DiagnosticImport", "OrdemServicoId");
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "DiagnosticImport");
}
