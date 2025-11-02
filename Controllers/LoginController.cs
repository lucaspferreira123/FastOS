using MeuProjeto.Business;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace TesteProjeto.Controllers
{
    public class LoginController : Controller
    {
        private readonly UsuarioBusiness _usuarioBusiness;

        public LoginController(UsuarioBusiness usuarioBusiness)
        {
            _usuarioBusiness = usuarioBusiness;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("Login/VerificarLoginUsuarioAoLogar")]
        public async Task<IActionResult> VerificarLoginUsuarioAoLogar([FromBody] LoginRequest usuario)
        {
            try
            {
                var usuarios = await _usuarioBusiness.ObterUsuarioPeloLoginESenha(usuario.Email, usuario.Password);

                if (usuarios == null || !usuarios.Any())
                {
                    return Unauthorized("Email ou senha incorretos.");
                }

                return Ok(usuarios); 
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno."); 
            }
        }
    }
}

