using FastOS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using FastOS.Domain.Entities;

namespace FastOS.Application.Services
{
    public class UsuarioBusiness
    {
        private readonly UsuarioRepository _repository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsuarioBusiness(
            UsuarioRepository repository,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<string> VerificarLoginUsuarioAoLogar(string email, string senha)
        {
            var usuarioValidado = await ObterUsuarioPeloLoginESenha(email, senha);

            if (!usuarioValidado.Ativo)
            {
                throw new UnauthorizedAccessException("Usuário ou senha incorreta.");
            }

            var identityUser = await _userManager.FindByEmailAsync(usuarioValidado.Email);
            if (identityUser == null)
            {
                identityUser = new ApplicationUser
                {
                    UserName = usuarioValidado.Email,
                    Email = usuarioValidado.Email,
                    Nome = usuarioValidado.Nome,
                    Ativo = usuarioValidado.Ativo,
                    EmailConfirmed = true
                };

                var resultadoCriacao = await _userManager.CreateAsync(identityUser);
                if (!resultadoCriacao.Succeeded)
                {
                    throw new InvalidOperationException("Não foi possível preparar o acesso do usuário.");
                }
            }
            else
            {
                identityUser.UserName = usuarioValidado.Email;
                identityUser.Email = usuarioValidado.Email;
                identityUser.Nome = usuarioValidado.Nome;
                identityUser.Ativo = usuarioValidado.Ativo;

                var resultadoAtualizacao = await _userManager.UpdateAsync(identityUser);
                if (!resultadoAtualizacao.Succeeded)
                {
                    throw new InvalidOperationException("Não foi possível preparar o acesso do usuário.");
                }
            }

            await _signInManager.SignInAsync(identityUser, isPersistent: false);

            return "/Home/Index";
        }

        public async Task<UsuarioEntity> ObterUsuarioPeloLoginESenha(string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                throw new ArgumentException("Email e senha não podem ser nulos ou vazios.");
            }

            var usuarios = await _repository.ObterUsuarioPeloLoginESenha(email, senha);
            var usuario = usuarios?.FirstOrDefault();

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Usuário ou senha incorreta.");
            }

            var passwordHasher = new PasswordHasher<UsuarioEntity>();

            var resultado = passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.Senha, 
                senha          
            );

            if (resultado == PasswordVerificationResult.Success ||
                resultado == PasswordVerificationResult.SuccessRehashNeeded)
            {
                return usuario;
            }

            throw new UnauthorizedAccessException("Usuário ou senha incorreta.");
        }

        public async Task<UsuarioEntity> CadastrarUsuario(UsuarioEntity usuario)
        {
            try
            {
                if (usuario == null)
                {
                    throw new ArgumentException("Não foi possivel cadastrar o usuario.");
                }

                var usuarios = await ObterUsuarioPeloNome(usuario.Nome);

                if (usuarios == null || !usuarios.Any())
                {
                    var passwordHasher = new PasswordHasher<UsuarioEntity>();

                    usuario.Senha = passwordHasher.HashPassword(usuario, usuario.Senha);
                    usuario.Excluido = false;
                    
                    var usuariosCadastrados = await _repository.CadastrarUsuario(usuario);

                    return usuariosCadastrados;
                }
                else
                {
                    throw new ArgumentException("Usuario já cadastrado!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao cadastrar usuario", ex);
            }
        }

        public async Task<List<UsuarioEntity>> ObterUsuarioPeloNome(string nome)
        {
            try
            {
                if (nome.IsNullOrEmpty())
                {
                    throw new Exception("Erro ao obter o usuario");
                }

                var usuarios = await _repository.ObterUsuarioPeloNome(nome);

                return usuarios;

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter os usuarios", ex);
            }
        }

        public async Task<UsuarioEntity> AlterarUsuario(UsuarioEntity usuario)
        {
            try
            {
                if (usuario == null)
                {
                    throw new ArgumentException("Não foi possivel alterar o usuario.");
                }

                var usuarioAntigo = ObterUsuarioPeloId(usuario.Id).Result.FirstOrDefault();

                if (usuarioAntigo == null)
                {
                    throw new ArgumentException("Usuario não encontrado para alteração!");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(usuario.Senha))
                    {
                        usuario.Senha = usuarioAntigo.Senha;
                    }
                    else
                    {
                        var passwordHasher = new PasswordHasher<UsuarioEntity>();
                        usuario.Senha = passwordHasher.HashPassword(usuario, usuario.Senha);
                    }

                    usuario.Excluido = usuarioAntigo.Excluido;

                    var usuarioAlterado = await _repository.AlterarUsuario(usuario);

                    return usuarioAlterado;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao alterar usuario", ex);
            }
        }

        public async Task<List<UsuarioEntity>> ObterTodosUsuarios()
        {
            try
            {
                var usuarios = await _repository.ObterTodosUsuarios();

                return usuarios;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao cadastrar usuarios", ex);
            }
        }

        public async Task<UsuarioEntity> ExcluirUsuario(int idUsuario)
        {
            try
            {
                if (idUsuario == 0)
                {
                    throw new Exception("Erro ao cadastrar usuario");
                }

                var usuarioExcluido = await _repository.ExcluirUsuario(idUsuario);

                return usuarioExcluido;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao excluir usuarios", ex);
            }
        }

        public async Task<List<UsuarioEntity>> ObterUsuarioPeloId(int idUsuario)
        {
            try
            {

                var usuario = await _repository.ObterUsuarioPeloId(idUsuario);

                return usuario;

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter o usuario", ex);
            }
        }
    }
}

