using MeuProjeto.Business;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TesteMVC.Models;

namespace TesteProjeto.Controllers
{
    public class OrdemController : Controller
    {
        private readonly OrdemBusiness _ordemBusiness;

        public OrdemController(OrdemBusiness ordemBusiness)
        {
            _ordemBusiness = ordemBusiness;
        }

        [HttpPost]
        [Route("Ordem/CadastrarOrdem")]
        public async Task<IActionResult> CadastrarOrdem([FromBody] OrdemServicoViewModel ordem)
        {
            try
            {
                var ordemCadastrada = await _ordemBusiness.CadastrarOrdem(ordem);

                return Ok(ordemCadastrada);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }

        [HttpPost]
        [Route("Ordem/ObterOrdens")]
        public async Task<IActionResult> ObterTodasOrdens()
        {
            try
            {
                var ordens = await _ordemBusiness.ObterTodasOrdens();

                return Ok(ordens);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }
    }
}

