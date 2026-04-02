using MeuProjeto.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TesteMVC.Models;

namespace TesteProjeto.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UsuarioBusiness _usuarioBusiness;

        public LoginController(
            SignInManager<ApplicationUser> signInManager,
            UsuarioBusiness usuarioBusiness)
        {
            _signInManager = signInManager;
            _usuarioBusiness = usuarioBusiness;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login/VerificarLoginUsuarioAoLogar")]
        public async Task<IActionResult> VerificarLoginUsuarioAoLogar([FromBody] LoginRequest usuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario.Email) || string.IsNullOrWhiteSpace(usuario.Password))
                {
                    return BadRequest("Email e senha são obrigatórios.");
                }

                var redirectUrl = await _usuarioBusiness.VerificarLoginUsuarioAoLogar(usuario.Email, usuario.Password);
                return Ok(new { redirectUrl });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Email ou senha incorretos.");
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

        [HttpPost]
        [Authorize]
        [Route("Login/Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}
