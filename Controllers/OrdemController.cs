using MeuProjeto.Business;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TesteMVC.Models;

namespace TesteProjeto.Controllers
{
    public class OrdemController : Controller
    {
        private readonly OrdemServicoBusiness _ordemBusiness;

        public OrdemController(OrdemServicoBusiness ordemBusiness)
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

                return Ok(new
                {
                    id = ordemCadastrada.idOrdemServico,
                    mensagem = "Criado com sucesso"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }

        [HttpGet]
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

        [HttpGet]
        [Route("Ordem/ObterOrdem/{idOrdem}")]
        public async Task<IActionResult> ObterOrdem(int idOrdem)
        {
            try
            {
                var ordem = await _ordemBusiness.ObterOrdemDto(idOrdem);

                return Ok(ordem);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }

        [HttpPut]
        [Route("Ordem/AlterarOrdem")]
        public async Task<IActionResult> AlterarOrdem([FromBody] OrdemServicoViewModel model)
        {
            try
            {
                var ordem = await _ordemBusiness.AlterarOrdemServico(model);

                return Ok(ordem);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }

        [HttpPut]
        [Route("Ordem/AlterarStatusOrdem/{idOrdem}/{idStatus}")]
        public async Task<IActionResult> AlterarStatusOrdem(int idOrdem, int idStatus)
        {
            try
            {
                await _ordemBusiness.AlterarStatusOrdem(idOrdem, idStatus);

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }
    }
}

