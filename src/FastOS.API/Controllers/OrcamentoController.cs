using FastOS.Application.Services;
using FastOS.Domain.Entities;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace FastOS.API.Controllers
{
    public class OrcamentoController : Controller
    {
        private readonly OrcamentoBusiness _orcamentoBusiness;

        public OrcamentoController(OrcamentoBusiness orcamentoBusiness)
        {
            _orcamentoBusiness = orcamentoBusiness;
        }

        [HttpPost]
        [Route("Orcamento/AlterarOrcamento")]
        public async Task<IActionResult> AlterarOrcamento([FromBody] OrcamentoEntity orcamento)
        {
            try
            {
                var orcamentoAlterado = await _orcamentoBusiness.AlterarOrcamento(orcamento);

                return Ok(orcamentoAlterado);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }

        [HttpGet]
        [Route("Orcamento/ObterOrcamento/{idOrdem}")]
        public async Task<IActionResult> ObterOrcamento(int idOrdem)
        {
            try
            {
                var orcamento = await _orcamentoBusiness.ObterOrcamento(idOrdem);

                return Ok(orcamento);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }
    }
}


