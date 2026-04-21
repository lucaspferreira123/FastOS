using FastOS.Application.Services;
using FastOS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FastOS.API.Controllers;

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
            var itens = await _itemOrdemServicoBusiness.ObterItensOrdemServico(idOrdem);
            return Ok(itens);
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocorreu um erro interno.");
        }
    }
}
