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
        return _context.Usuario
            .Where(u => !u.Excluido)
            .ToList();
    }

    public async Task<List<UsuarioEntity>> ObterUsuarioPeloLoginESenha(string email, string senha)
    {
        try
        {
            return await _context.Usuario
                .Where(u => u.Ativo && !u.Excluido && u.Email == email)
                .Select(u => new UsuarioEntity
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    Senha = u.Senha,
                    Ativo = u.Ativo,
                    Excluido = u.Excluido,
                    Cargo = u.Cargo
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao buscar usuario pelo login e senha", ex);
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
            return await _context.Usuario
                .Where(u => !u.Excluido)
                .ToListAsync();
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
            return await _context.Usuario
                .FromSqlRaw("SELECT * FROM Usuario WHERE Nome = @nome", param)
                .Where(u => !u.Excluido)
                .ToListAsync();
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
            return await _context.Usuario
                .FromSqlRaw("SELECT * FROM Usuario WHERE Id = @idUsuario", param)
                .Where(u => !u.Excluido)
                .ToListAsync();
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
            var usuarioExistente = await _context.Usuario
                .FirstOrDefaultAsync(u => u.Id == dadosAtualizados.Id && !u.Excluido);

            if (usuarioExistente == null)
            {
                throw new Exception("Usuario nao encontrado.");
            }

            usuarioExistente.Nome = dadosAtualizados.Nome;
            usuarioExistente.Email = dadosAtualizados.Email;
            usuarioExistente.Senha = dadosAtualizados.Senha;
            usuarioExistente.Ativo = dadosAtualizados.Ativo;
            usuarioExistente.Excluido = dadosAtualizados.Excluido;
            usuarioExistente.Cargo = dadosAtualizados.Cargo;

            _context.Usuario.Update(usuarioExistente);
            await _context.SaveChangesAsync();
            return usuarioExistente;
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao atualizar usuario", ex);
        }
    }

    public async Task<UsuarioEntity> ExcluirUsuario(int idUsuario)
    {
        try
        {
            var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Id == idUsuario && !u.Excluido);
            if (usuario == null)
            {
                throw new Exception("Usuario nao encontrado.");
            }

            usuario.Ativo = false;
            usuario.Excluido = true;
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
