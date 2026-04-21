using FastOS.Application.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using FastOS.Domain.Interfaces;
using FastOS.Domain.Entities;
using FastOS.Application.Reports;

namespace FastOS.API.Controllers
{
    public class RelatorioController : Controller
    {
        private readonly RelatorioBusiness _relatorioBusiness;

        public RelatorioController(RelatorioBusiness relatorioBusiness)
        {
            _relatorioBusiness = relatorioBusiness;
        }

        [HttpGet]
        [Route("Relatorio/ImprimirRelatorioOrcamento/{idOrdem}")]
        public async Task<IActionResult> ImprimirRelatorioOrcamento(int idOrdem)
        {
            try
            {
                var model = await _relatorioBusiness.PopularRelatorioOrcamento(idOrdem);

                var relatorioModel = new RelatorioOrcamentoDocument(model);

                var pdf = relatorioModel.GeneratePdf();

                return File(pdf, "application/pdf", $"Relatorio_Orcamento_{idOrdem}.pdf");
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }
    }
}


