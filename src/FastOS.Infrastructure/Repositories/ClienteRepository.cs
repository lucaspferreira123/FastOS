using FastOS.Domain.Entities;
using FastOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace FastOS.Infrastructure.Repositories;

public class ClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ClienteEntity> CadastrarCliente(ClienteEntity clienteVm)
    {
        try
        {
            _context.Cliente.Add(clienteVm);
            await _context.SaveChangesAsync();
            return clienteVm;
        }
        catch (DbUpdateException ex)
        {
            var innerMessage = ex.InnerException?.Message;
            throw new Exception($"Erro ao salvar cliente: {innerMessage}", ex);
        }
    }

    public async Task<List<ClienteEntity>> ObterTodosClientes()
    {
        try
        {
            return await _context.Cliente
                .Where(c => c.Ativo && !c.Excluido)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao obter os clientes", ex);
        }
    }

    public async Task<List<ClienteEntity>> ObterClientePeloNome(string nome)
    {
        try
        {
            var param = new SqlParameter("@nome", nome);
            return await _context.Cliente
                .FromSqlRaw("SELECT * FROM Cliente WHERE nome = @nome AND excluido = 0", param)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao obter cliente", ex);
        }
    }

    public async Task<List<ClienteEntity>> ObterClientePeloId(int idCliente)
    {
        try
        {
            var param = new SqlParameter("@idCliente", idCliente);
            return await _context.Cliente
                .FromSqlRaw("SELECT * FROM Cliente WHERE idCliente = @idCliente AND excluido = 0", param)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao obter cliente", ex);
        }
    }

    public async Task<ClienteEntity> AlterarCliente(ClienteEntity dadosAtualizados)
    {
        try
        {
            var clienteExistente = await _context.Cliente.FromSqlRaw("SELECT * FROM Cliente WHERE idCliente = @idCliente AND excluido = 0",
                new SqlParameter("@idCliente", dadosAtualizados.idCliente)).FirstOrDefaultAsync();

            if (clienteExistente == null)
                throw new Exception("Cliente não encontrado.");

            clienteExistente.Nome = dadosAtualizados.Nome;
            clienteExistente.Email = dadosAtualizados.Email;
            clienteExistente.Telefone = dadosAtualizados.Telefone;
            clienteExistente.Endereco = dadosAtualizados.Endereco;
            clienteExistente.Ativo = dadosAtualizados.Ativo;
            clienteExistente.TipoCliente = dadosAtualizados.TipoCliente;
            clienteExistente.CNPJ = dadosAtualizados.CNPJ;
            clienteExistente.CPF = dadosAtualizados.CPF;

            _context.Cliente.Update(clienteExistente);
            await _context.SaveChangesAsync();

            return clienteExistente;
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao atualizar cliente", ex);
        }
    }

    public async Task<ClienteEntity> ExcluirCliente(int idCliente)
    {
        try
        {
            var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.idCliente == idCliente && !c.Excluido);

            if (cliente == null)
                throw new Exception("Cliente não encontrado.");

            cliente.Ativo = false;
            cliente.Excluido = true;
            _context.Cliente.Update(cliente);
            await _context.SaveChangesAsync();

            return cliente;
        }
        catch (DbUpdateException ex)
        {
            var innerMessage = ex.InnerException?.Message;
            throw new Exception($"Erro ao excluir cliente: {innerMessage}", ex);
        }
    }
}
