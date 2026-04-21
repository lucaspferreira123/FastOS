using FastOS.Domain.Entities;
using FastOS.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FastOS.Infrastructure.Repositories;

public class UsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<UsuarioEntity> GetAll()
    {
        return _context.Usuario.ToList();
    }

    public async Task<List<UsuarioEntity>> ObterUsuarioPeloLoginESenha(string email, string senha)
    {
        try
        {
            return await _context.Usuario
                .Where(u => u.Ativo && u.Email == email)
                .Select(u => new UsuarioEntity
                {
                    idUsuario = u.idUsuario,
                    Nome = u.Nome,
                    Email = u.Email,
                    Senha = u.Senha,
                    Ativo = u.Ativo
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao buscar usuário pelo login e senha", ex);
        }
    }

    public async Task<UsuarioEntity> CadastrarUsuario(UsuarioEntity usuario)
    {
        try
        {
            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }
        catch (DbUpdateException ex)
        {
            var innerMessage = ex.InnerException?.Message;
            throw new Exception($"Erro ao salvar usuario: {innerMessage}", ex);
        }
    }

    public async Task<List<UsuarioEntity>> ObterTodosUsuarios()
    {
        try
        {
            return await _context.Usuario.FromSqlRaw(@" SELECT * FROM Usuario").ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao obter os usuarios", ex);
        }
    }

    public async Task<List<UsuarioEntity>> ObterUsuarioPeloNome(string nome)
    {
        try
        {
            var param = new SqlParameter("@nome", nome);
            return await _context.Usuario.FromSqlRaw("SELECT * FROM Usuario WHERE Nome = @nome", param).ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao obter usuario", ex);
        }
    }

    public async Task<List<UsuarioEntity>> ObterUsuarioPeloId(int idUsuario)
    {
        try
        {
            var param = new SqlParameter("@idUsuario", idUsuario);
            return await _context.Usuario.FromSqlRaw("SELECT * FROM Usuario WHERE idUsuario = @idUsuario", param).ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao obter usuario", ex);
        }
    }

    public async Task<UsuarioEntity> AlterarUsuario(UsuarioEntity dadosAtualizados)
    {
        try
        {
            var usuarioExistente = await _context.Usuario.FromSqlRaw("SELECT * FROM Usuario WHERE idUsuario = @idUsuario",
                new SqlParameter("@idUsuario", dadosAtualizados.idUsuario)).FirstOrDefaultAsync();

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

    public async Task<UsuarioEntity> ExcluirUsuario(int idUsuario)
    {
        try
        {
            var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.idUsuario == idUsuario);
            if (usuario == null)
                throw new Exception("Usuario não encontrado.");

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
