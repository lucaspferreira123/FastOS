using MeuProjeto.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TesteMVC.Models;

namespace MeuProjeto.Business
{
    public class ClienteBusiness
    {
        private readonly ClienteRepository _repository;

        public ClienteBusiness(ClienteRepository repository)
        {
            _repository = repository;
        }

        public async Task<ClienteViewModel> CadastrarCliente(ClienteViewModel cliente)
        {
            try
            {
                if (cliente == null)
                {
                    throw new ArgumentException("Não foi possivel cadastrar o cliente.");
                }

                var clientes = await ObterClientePeloNome(cliente.Nome);

                if (clientes == null || !clientes.Any())
                {
                    var clientesCadastrados = await _repository.CadastrarCliente(cliente);

                    return clientesCadastrados;
                }
                else
                {
                    throw new ArgumentException("Cliente já cadastrado!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao cadastrar cliente", ex);
            }
        }

        public async Task<ClienteViewModel> AlterarCliente(ClienteViewModel cliente)
        {
            try
            {
                if (cliente == null)
                {
                    throw new ArgumentException("Não foi possivel alterar o cliente.");
                }

                var clienteAntigo = ObterClientePeloId(cliente.idCliente).Result.FirstOrDefault();

                if (clienteAntigo == null)
                {
                    throw new ArgumentException("Cliente não encontrado para alteração!");
                }
                else
                {
                    var clienteAlterado = await _repository.AlterarCliente(cliente);

                    return clienteAlterado;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao alterar cliente", ex);
            }
        }

        public async Task<List<ClienteViewModel>> ObterClientePeloNome(string nome)
        {
            try
            {
                if (nome.IsNullOrEmpty())
                {
                    throw new Exception("Erro ao obter o Cliente");
                }

                var cliente = await _repository.ObterClientePeloNome(nome);

                return cliente;

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter o Cliente", ex);
            }
        }

        public async Task<List<ClienteViewModel>> ObterClientePeloId(int idCliente)
        {
            try
            {

                var cliente = await _repository.ObterClientePeloId(idCliente);

                return cliente;

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter o Cliente", ex);
            }
        }

        public async Task<List<ClienteViewModel>> ObterTodosClientes()
        {
            try
            {
                var clientes = await _repository.ObterTodosClientes();

                return clientes;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao cadastrar cliente", ex);
            }
        }

        public async Task<ClienteViewModel> ExcluirCliente(int idCliente)
        {
            try
            {
                if (idCliente == 0)
                {
                    throw new Exception("Erro ao cadastrar cliente");
                }

                var clienteExcluido = await _repository.ExcluirCliente(idCliente);

                return clienteExcluido;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao excluir cliente", ex);
            }
        }
    }
}