using MeuProjeto.Business;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TesteMVC.Models;

namespace TesteProjeto.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioBusiness _usuarioBusiness;

        public UsuarioController(UsuarioBusiness usuarioBusiness)
        {
            _usuarioBusiness = usuarioBusiness;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("Usuario/CadastrarUsuario")]
        public async Task<IActionResult> CadastrarUsuario([FromBody] UsuarioViewModel usuario)
        {
            try
            {
                var usuarioCadastrado = await _usuarioBusiness.CadastrarUsuario(usuario);

                return Ok(usuarioCadastrado);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }

        [HttpPut]
        [Route("Usuario/AlterarUsuario")]
        public async Task<IActionResult> AlterarUsuario([FromBody] UsuarioViewModel usuario)
        {
            try
            {
                var usuarioAlterado = await _usuarioBusiness.AlterarUsuario(usuario);

                return Ok(usuarioAlterado);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }

        [HttpDelete]
        [Route("Usuario/ExcluirUsuario/{idUsuario}")]
        public async Task<IActionResult> ExcluirUsuario(int idUsuario)
        {
            try
            {
                var usuarioExcluido = await _usuarioBusiness.ExcluirUsuario(idUsuario);

                return Ok(usuarioExcluido);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno.");
            }
        }
    }
}

