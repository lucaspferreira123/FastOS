using FastOS.Application.Services;
using FastOS.Domain.Entities;
using FastOS.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace FastOS.API.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ClienteBusiness _clienteBusiness;
        private readonly OrdemServicoBusiness _ordemBusiness;
        private readonly OrcamentoBusiness _orcamentoBusiness;

        public ClienteController(ClienteBusiness clienteBusiness, OrdemServicoBusiness ordemBusiness, OrcamentoBusiness orcamentoBusiness)
        {
            _clienteBusiness   = clienteBusiness;
            _ordemBusiness     = ordemBusiness;
            _orcamentoBusiness = orcamentoBusiness;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("Cliente/Perfil/{idCliente:int}")]
        public IActionResult Perfil(int idCliente)
        {
            return View();
        }

        [HttpGet]
        [Route("Cliente/ObterPerfil/{idCliente}")]
        public async Task<IActionResult> ObterPerfil(int idCliente)
        {
            try
            {
                var cliente = (await _clienteBusiness.ObterClientePeloId(idCliente)).FirstOrDefault();
                if (cliente == null) return NotFound();

                var todasOrdens = await _ordemBusiness.ObterTodasOrdens();
                var ordensCliente = todasOrdens.Where(o => o.idCliente == idCliente).OrderByDescending(o => o.DataAbertura).ToList();

                var idsStatusConcluido = new[] { 5, 6 };
                var agora = DateTime.Now;

                // Busca orçamentos das OS concluídas para calcular total gasto
                decimal totalGasto = 0;
                foreach (var os in ordensCliente.Where(o => idsStatusConcluido.Contains(o.idStatus)))
                {
                    try
                    {
                        var orc = await _orcamentoBusiness.ObterOrcamento(os.idOrdemServico);
                        if (orc != null) totalGasto += orc.ValorFinal;
                    }
                    catch { /* orçamento não cadastrado, ignora */ }
                }

                var osConcluidas = ordensCliente.Count(o => idsStatusConcluido.Contains(o.idStatus));

                // Equipamentos = descrições únicas das OS (primeiras palavras)
                var equipamentos = ordensCliente
                    .Select(o => o.DescricaoServico?.Split('-').FirstOrDefault()?.Trim() ?? o.DescricaoServico)
                    .Where(d => !string.IsNullOrWhiteSpace(d))
                    .Distinct()
                    .Take(20)
                    .ToList();

                var perfil = new ClientePerfilDto
                {
                    IdCliente    = cliente.idCliente,
                    Nome         = cliente.Nome,
                    Email        = cliente.Email,
                    Telefone     = cliente.Telefone,
                    Endereco     = cliente.Endereco,
                    TipoCliente  = cliente.TipoCliente == Domain.Enums.TipoClienteEnum.PessoaFisica ? "Pessoa Física" : "Pessoa Jurídica",
                    Documento    = cliente.TipoCliente == Domain.Enums.TipoClienteEnum.PessoaFisica ? cliente.CPF : cliente.CNPJ,
                    Ativo        = cliente.Ativo,
                    TotalOS      = ordensCliente.Count,
                    OSAbertas    = ordensCliente.Count(o => !idsStatusConcluido.Contains(o.idStatus)),
                    OSConcluidas = osConcluidas,
                    OSAtrasadas  = ordensCliente.Count(o => !idsStatusConcluido.Contains(o.idStatus) && o.PrevisaoEntrega < agora),
                    TotalGasto   = totalGasto,
                    TicketMedio  = osConcluidas > 0 ? Math.Round(totalGasto / osConcluidas, 2) : 0,
                    Ordens       = ordensCliente,
                    Equipamentos = equipamentos!
                };

                return Ok(perfil);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
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
                    c.Ativo,
                    c.Excluido,
                    c.TipoCliente,
                    c.CNPJ,
                    c.CPF
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
                    cliente.Ativo,
                    cliente.Excluido,
                    cliente.TipoCliente,
                    cliente.CNPJ,
                    cliente.CPF
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
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }
    }
}
