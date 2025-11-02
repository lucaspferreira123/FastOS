using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TesteMVC.Models;

namespace MeuProjeto.Repository
{
    public class UsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<UsuarioViewModel> GetAll()
        {
            return _context.Usuario.ToList();
        }

        public async Task<List<UsuarioViewModel>> ObterUsuarioPeloLoginESenha(string email, string senha)
        {
            try
            {
                var usuarios = await _context.Usuario
                    .Where(u => u.Ativo == true && u.Email == email && u.Senha == senha)
                    .Select(u => new UsuarioViewModel
                    {
                        idUsuario = u.idUsuario,
                        Nome = u.Nome,
                        Email = u.Email,
                    })
                    .ToListAsync();

                return usuarios;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar usuário pelo login e senha", ex);
            }
        }
        public async Task<UsuarioViewModel> CadastrarUsuario(UsuarioViewModel usuario)
        {
            try
            {

                _context.Usuario.Add(usuario);

                await _context.SaveChangesAsync();

                var usuarioCadastrado = usuario;

                return usuarioCadastrado;
            }
            catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message;
                throw new Exception($"Erro ao salvar usuario: {innerMessage}", ex);
            }
        }


        public async Task<List<UsuarioViewModel>> ObterTodosUsuarios()
        {
            try
            {
                var usuarios = await _context.Usuario.FromSqlRaw(@" SELECT * FROM Usuario").ToListAsync();

                return usuarios;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter os usuarios", ex);
            }
        }

        public async Task<List<UsuarioViewModel>> ObterUsuarioPeloNome(string nome)
        {
            try
            {
                var param = new SqlParameter("@nome", nome);

                var usuarios = await _context.Usuario.FromSqlRaw("SELECT * FROM Usuario WHERE Nome = @nome", param).ToListAsync();

                return usuarios;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter usuario", ex);
            }
        }

        public async Task<List<UsuarioViewModel>> ObterUsuarioPeloId(int idUsuario)
        {
            try
            {
                var param = new SqlParameter("@idUsuario", idUsuario);

                var usuarios = await _context.Usuario.FromSqlRaw("SELECT * FROM Usuario WHERE idUsuario = @idUsuario", param).ToListAsync();

                return usuarios;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter usuario", ex);
            }
        }

        public async Task<UsuarioViewModel> AlterarUsuario(UsuarioViewModel dadosAtualizados)
        {
            try
            {
                var usuarioExistente = await _context.Usuario
                    .FromSqlRaw("SELECT * FROM Usuario WHERE idUsuario = @idUsuario",
                        new SqlParameter("@idUsuario", dadosAtualizados.idUsuario))
                    .FirstOrDefaultAsync();

                if (usuarioExistente == null)
                    throw new Exception("Usuário não encontrado.");

                usuarioExistente.Nome = dadosAtualizados.Nome;
                usuarioExistente.Email = dadosAtualizados.Email;
                usuarioExistente.Senha = dadosAtualizados.Senha;
                usuarioExistente.Ativo = dadosAtualizados.Ativo;

                _context.Usuario.Update(usuarioExistente);
                await _context.SaveChangesAsync();

                return usuarioExistente;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar usuário", ex);
            }
        }

        public async Task<UsuarioViewModel> ExcluirUsuario(int idUsuario)
        {
            try
            {
                var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.idUsuario == idUsuario);

                if (usuario == null)
                {
                    throw new Exception("Usuario não encontrado.");
                }

                usuario.Ativo = false;

                _context.Usuario.Update(usuario);

                await _context.SaveChangesAsync();

                return usuario;
            }
            catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message;
                throw new Exception($"Erro ao excluir usuario: {innerMessage}", ex);
            }
        }
    }
}
