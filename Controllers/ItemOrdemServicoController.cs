using MeuProjeto.Business;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TesteMVC.Models;

namespace TesteProjeto.Controllers
{
    public class ItemOrdemServicoController : Controller
    {
        private readonly ItemOrdemServicoBusiness _itemOrdemServicoBusiness;

        public ItemOrdemServicoController(ItemOrdemServicoBusiness itemOrdemServicoBusiness)
        {
            _itemOrdemServicoBusiness = itemOrdemServicoBusiness;
        }

        [HttpPost]
        [Route("ItemOrdemServico/AlterarItensOrdemServico")]
        public async Task<IActionResult> AlterarItensOrdemServico([FromBody] List<ItemOrdemServicoViewModel> itens)
        {
            try
            {
                var itensCadastrados = await _itemOrdemServicoBusiness.AlterarItensOrdemServico(itens);

                return Ok(itensCadastrados);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }

        [HttpGet]
        [Route("ItemOrdemServico/ObterItensOrdemServico/{idOrdem}")]
        public async Task<IActionResult> ObterItensOrdemServico(int idOrdem)
        {
            try
            {
                var itens = await _itemOrdemServicoBusiness.ObterItensOrdemServico();

                return Ok(itens);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }
    }
}

