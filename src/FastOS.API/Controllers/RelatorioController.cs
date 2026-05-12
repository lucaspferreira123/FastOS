using FastOS.Application.Services;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using FastOS.Application.Reports;

namespace FastOS.API.Controllers
{
    public class RelatorioController : Controller
    {
        private readonly RelatorioBusiness _relatorioBusiness;
        private readonly EmailService _emailService;
        private readonly ClienteBusiness _clienteBusiness;

        public RelatorioController(RelatorioBusiness relatorioBusiness, EmailService emailService, ClienteBusiness clienteBusiness)
        {
            _relatorioBusiness = relatorioBusiness;
            _emailService      = emailService;
            _clienteBusiness   = clienteBusiness;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Relatorio/ObterDadosDashboard")]
        public async Task<IActionResult> ObterDadosDashboard()
        {
            try
            {
                var dados = await _relatorioBusiness.ObterDadosDashboard();
                return Ok(dados);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
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

        [HttpGet]
        [Route("Relatorio/ImprimirRecibo/{idOrdem}")]
        public async Task<IActionResult> ImprimirRecibo(int idOrdem)
        {
            try
            {
                var model = await _relatorioBusiness.PopularRelatorioOrcamento(idOrdem);
                var recibo = new ReciboCobrancaDocument(model);
                var pdf = recibo.GeneratePdf();
                return File(pdf, "application/pdf", $"Recibo_OS_{idOrdem}.pdf");
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }

        [HttpPost]
        [Route("Relatorio/EnviarReciboPorEmail/{idOrdem}")]
        public async Task<IActionResult> EnviarReciboPorEmail(int idOrdem)
        {
            try
            {
                // Gera o PDF
                var model = await _relatorioBusiness.PopularRelatorioOrcamento(idOrdem);

                if (model == null)
                    return BadRequest("Ordem de serviço não encontrada.");

                if (model.Orcamento == null)
                    return BadRequest("Esta ordem não possui orçamento cadastrado. Cadastre um orçamento antes de enviar o recibo.");

                if (model.OrdemServico == null)
                    return BadRequest("Dados da ordem de serviço não encontrados.");

                var recibo = new ReciboCobrancaDocument(model);
                var pdf    = recibo.GeneratePdf();

                // Busca e-mail do cliente diretamente pelo idCliente da entidade OS
                var ordemEntity = await _relatorioBusiness.ObterIdClienteDaOrdem(idOrdem);
                var clientes    = await _clienteBusiness.ObterClientePeloId(ordemEntity);
                var cliente     = clientes?.FirstOrDefault();

                if (cliente == null)
                    return BadRequest("Cliente não encontrado.");

                if (string.IsNullOrWhiteSpace(cliente.Email))
                    return BadRequest("Cliente não possui e-mail cadastrado.");

                await _emailService.EnviarReciboAsync(
                    destinatario: cliente.Email,
                    nomeCliente:  cliente.Nome,
                    idOrdem:      idOrdem,
                    pdfBytes:     pdf
                );

                return Ok(new { mensagem = $"Recibo enviado para {cliente.Email} com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao enviar e-mail: {ex.Message}");
            }
        }
    }
}


