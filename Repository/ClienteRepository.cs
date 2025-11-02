using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TesteMVC.Models;

namespace MeuProjeto.Repository
{
    public class ClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ClienteViewModel> CadastrarCliente(ClienteViewModel clienteVm)
        {
            try
            {
                var cliente = clienteVm;

                _context.Cliente.Add(cliente);

                await _context.SaveChangesAsync();

                var clienteCadastrado = clienteVm;

                return clienteCadastrado;
            }
            catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message;
                throw new Exception($"Erro ao salvar cliente: {innerMessage}", ex);
            }
        }


        public async Task<List<ClienteViewModel>> ObterTodosClientes()
        {
            try
            {
                var clientes = await _context.Cliente.FromSqlRaw(@" SELECT * FROM Cliente WHERE excluido = 0").ToListAsync();

                return clientes;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter os clientes", ex);
            }
        }

        public async Task<List<ClienteViewModel>> ObterClientePeloNome(string nome)
        {
            try
            {
                var param = new SqlParameter("@nome", nome);

                var clientes = await _context.Cliente.FromSqlRaw("SELECT * FROM Cliente WHERE nome = @nome", param).ToListAsync();

                return clientes;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter cliente", ex);
            }
        }

        public async Task<List<ClienteViewModel>> ObterClientePeloId(int idCliente)
        {
            try
            {
                var param = new SqlParameter("@idCliente", idCliente);

                var clientes = await _context.Cliente.FromSqlRaw("SELECT * FROM Cliente WHERE idCliente = @idCliente", param).ToListAsync();

                return clientes;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter cliente", ex);
            }
        }

        public async Task<ClienteViewModel> AlterarCliente(ClienteViewModel dadosAtualizados)
        {
            try
            {
                var clienteExistente = await _context.Cliente.FromSqlRaw("SELECT * FROM Cliente WHERE idCliente = @idCliente",
                    new SqlParameter("@idCliente", dadosAtualizados.idCliente)).FirstOrDefaultAsync();


                if (clienteExistente == null)
                    throw new Exception("Cliente não encontrado.");

                clienteExistente.Nome = dadosAtualizados.Nome;
                clienteExistente.Email = dadosAtualizados.Email;
                clienteExistente.Senha = dadosAtualizados.Senha;
                clienteExistente.Telefone = dadosAtualizados.Telefone;
                clienteExistente.Endereco = dadosAtualizados.Endereco;

                _context.Cliente.Update(clienteExistente);
                await _context.SaveChangesAsync();

                return clienteExistente;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar cliente", ex);
            }
        }

        public async Task<ClienteViewModel> ExcluirCliente(int idCliente)
        {
            try
            {
                var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.idCliente == idCliente);

                if (cliente == null)
                {
                    throw new Exception("Cliente não encontrado.");
                }

                _context.Cliente.Remove(cliente);

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
}
