using FastOS.Application.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using FastOS.Domain.Entities;

namespace FastOS.API.Controllers
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

        [HttpGet]
        [Route("Cliente/ObterClientes")]
        public async Task<IActionResult> ObterClientes()
        {
            try
            {
                var clientes = await _clienteBusiness.ObterTodosClientes();

                return Ok(clientes.Select(c => new
                {
                    c.idCliente,
                    c.Nome,
                    c.Email,
                    c.Telefone,
                    c.Endereco,
                    c.Ativo
                }));
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }

        [HttpGet]
        [Route("Cliente/ObterCliente/{idCliente}")]
        public async Task<IActionResult> ObterCliente(int idCliente)
        {
            try
            {
                var cliente = (await _clienteBusiness.ObterClientePeloId(idCliente)).FirstOrDefault();

                if (cliente == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    cliente.idCliente,
                    cliente.Nome,
                    cliente.Email,
                    cliente.Telefone,
                    cliente.Endereco,
                    cliente.Ativo
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }

        [HttpPost]
        [Route("Cliente/CadastrarCliente")]
        public async Task<IActionResult> CadastrarCliente([FromBody] ClienteEntity cliente)
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
        public async Task<IActionResult> AlterarCliente([FromBody] ClienteEntity cliente)
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


