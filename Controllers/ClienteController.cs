using MeuProjeto.Business;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TesteMVC.Models;

namespace TesteProjeto.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ClienteBusiness _clienteBusiness;

        public ClienteController(ClienteBusiness clienteBusiness)
        {
            _clienteBusiness = clienteBusiness;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("Cliente/CadastrarCliente")]
        public async Task<IActionResult> CadastrarCliente([FromBody] ClienteViewModel cliente)
        {
            try
            {
                var clienteCadastrado = await _clienteBusiness.CadastrarCliente(cliente);

                return Ok(clienteCadastrado);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }

        [HttpPut]
        [Route("Cliente/AlterarCliente")]
        public async Task<IActionResult> AlterarCliente([FromBody] ClienteViewModel cliente)
        {
            try
            {
                var clienteAlterado = await _clienteBusiness.AlterarCliente(cliente);

                return Ok(clienteAlterado);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }

        [HttpDelete("Cliente/ExcluirCliente/{idCliente}")]
        public async Task<IActionResult> ExcluirCliente(int idCliente)
        {
            try
            {
                var clienteExcluido = await _clienteBusiness.ExcluirCliente(idCliente);

                return Ok(clienteExcluido);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }
    }
}

