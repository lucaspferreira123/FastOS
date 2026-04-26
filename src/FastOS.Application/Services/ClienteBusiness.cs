using FastOS.Domain.Entities;
using FastOS.Domain.Enums;
using FastOS.Infrastructure.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace FastOS.Application.Services
{
    public class ClienteBusiness
    {
        private readonly ClienteRepository _repository;

        public ClienteBusiness(ClienteRepository repository)
        {
            _repository = repository;
        }

        public async Task<ClienteEntity> CadastrarCliente(ClienteEntity cliente)
        {
            try
            {
                if (cliente == null)
                {
                    throw new ArgumentException("Nao foi possivel cadastrar o cliente.");
                }

                PrepararCliente(cliente);
                var clientes = await ObterClientePeloNome(cliente.Nome);

                if (clientes == null || !clientes.Any())
                {
                    return await _repository.CadastrarCliente(cliente);
                }

                throw new ArgumentException("Cliente ja cadastrado.");
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao cadastrar cliente", ex);
            }
        }

        public async Task<ClienteEntity> AlterarCliente(ClienteEntity cliente)
        {
            try
            {
                if (cliente == null)
                {
                    throw new ArgumentException("Nao foi possivel alterar o cliente.");
                }

                var clienteAntigo = (await ObterClientePeloId(cliente.idCliente)).FirstOrDefault();

                if (clienteAntigo == null)
                {
                    throw new ArgumentException("Cliente nao encontrado para alteracao.");
                }

                PrepararCliente(cliente);
                return await _repository.AlterarCliente(cliente);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao alterar cliente", ex);
            }
        }

        public async Task<List<ClienteEntity>> ObterClientePeloNome(string nome)
        {
            try
            {
                if (nome.IsNullOrEmpty())
                {
                    throw new Exception("Erro ao obter o cliente");
                }

                return await _repository.ObterClientePeloNome(nome);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter o cliente", ex);
            }
        }

        public async Task<List<ClienteEntity>> ObterClientePeloId(int idCliente)
        {
            try
            {
                return await _repository.ObterClientePeloId(idCliente);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter o cliente", ex);
            }
        }

        public async Task<List<ClienteEntity>> ObterTodosClientes()
        {
            try
            {
                return await _repository.ObterTodosClientes();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter clientes", ex);
            }
        }

        public async Task<ClienteEntity> ExcluirCliente(int idCliente)
        {
            try
            {
                if (idCliente == 0)
                {
                    throw new ArgumentException("Cliente invalido.");
                }

                return await _repository.ExcluirCliente(idCliente);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao excluir cliente", ex);
            }
        }

        private static void PrepararCliente(ClienteEntity cliente)
        {
            cliente.Nome = cliente.Nome?.Trim() ?? string.Empty;
            cliente.Email = cliente.Email?.Trim() ?? string.Empty;
            cliente.Telefone = cliente.Telefone?.Trim() ?? string.Empty;
            cliente.Endereco = string.IsNullOrWhiteSpace(cliente.Endereco) ? null : cliente.Endereco.Trim();
            cliente.CPF = NormalizarDocumento(cliente.CPF);
            cliente.CNPJ = NormalizarDocumento(cliente.CNPJ);

            if (string.IsNullOrWhiteSpace(cliente.Nome))
            {
                throw new ArgumentException("Preencha a razao social do cliente.");
            }

            if (string.IsNullOrWhiteSpace(cliente.Email))
            {
                throw new ArgumentException("Preencha o email do cliente.");
            }

            if (string.IsNullOrWhiteSpace(cliente.Telefone))
            {
                throw new ArgumentException("Preencha o telefone do cliente.");
            }

            if (cliente.TipoCliente == TipoClienteEnum.PessoaJuridica)
            {
                if (string.IsNullOrWhiteSpace(cliente.CNPJ))
                {
                    throw new ArgumentException("Informe o CNPJ para cliente pessoa juridica.");
                }

                if (cliente.CNPJ.Length != 14)
                {
                    throw new ArgumentException("O CNPJ deve conter 14 numeros.");
                }

                cliente.CPF = null;
                return;
            }

            if (cliente.TipoCliente != TipoClienteEnum.PessoaFisica)
            {
                throw new ArgumentException("Tipo de cliente invalido.");
            }

            if (string.IsNullOrWhiteSpace(cliente.CPF))
            {
                throw new ArgumentException("Informe o CPF para cliente pessoa fisica.");
            }

            if (cliente.CPF.Length != 11)
            {
                throw new ArgumentException("O CPF deve conter 11 numeros.");
            }

            cliente.CNPJ = null;
        }

        private static string? NormalizarDocumento(string? documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
            {
                return null;
            }

            return new string(documento.Where(char.IsDigit).ToArray());
        }
    }
}
