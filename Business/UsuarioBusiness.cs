using MeuProjeto.Repository;
using Microsoft.IdentityModel.Tokens;
using TesteMVC.Models;

namespace MeuProjeto.Business
{
    public class UsuarioBusiness
    {
        private readonly UsuarioRepository _repository;

        public UsuarioBusiness(UsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<UsuarioViewModel>> ObterUsuarioPeloLoginESenha(string email, string senha)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                throw new ArgumentException("Email e senha não podem ser nulos ou vazios.");
            }
            else
            {
                var usuarios = await _repository.ObterUsuarioPeloLoginESenha(email, senha);

                if (usuarios == null || !usuarios.Any())
                {
                    throw new UnauthorizedAccessException("Usuário ou senha incorreta.");
                }
                
                return usuarios;
            }
        }

        public async Task<UsuarioViewModel> CadastrarUsuario(UsuarioViewModel usuario)
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

        public async Task<List<UsuarioViewModel>> ObterUsuarioPeloNome(string nome)
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

        public async Task<UsuarioViewModel> AlterarUsuario(UsuarioViewModel usuario)
        {
            try
            {
                if (usuario == null)
                {
                    throw new ArgumentException("Não foi possivel alterar o usuario.");
                }

                var usuarioAntigo = ObterUsuarioPeloId(usuario.idUsuario).Result.FirstOrDefault();

                if (usuarioAntigo == null)
                {
                    throw new ArgumentException("Usuario não encontrado para alteração!");
                }
                else
                {
                    var usuarioAlterado = await _repository.AlterarUsuario(usuario);

                    return usuarioAlterado;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao alterar usuario", ex);
            }
        }

        public async Task<List<UsuarioViewModel>> ObterTodosUsuarios()
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

        public async Task<UsuarioViewModel> ExcluirUsuario(int idUsuario)
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

        public async Task<List<UsuarioViewModel>> ObterUsuarioPeloId(int idUsuario)
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