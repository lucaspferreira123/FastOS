using FastOS.Domain.Entities;
using FastOS.Infrastructure.Repositories;

namespace FastOS.Application.Services
{
    public class ProdutoBusiness
    {
        private readonly ProdutoRepository _repository;

        public ProdutoBusiness(ProdutoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProdutoEntity> CadastrarProduto(ProdutoEntity produto)
        {
            try
            {
                if (produto == null)
                {
                    throw new ArgumentException("Não foi possível cadastrar o produto.");
                }

                var produtosExistentes = await ObterProdutoPeloCodigo(produto.Codigo);

                if (produtosExistentes == null || !produtosExistentes.Any())
                {
                    var produtoCadastrado = await _repository.CadastrarProduto(produto);
                    return produtoCadastrado;
                }

                throw new ArgumentException("Produto já cadastrado!");
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao cadastrar produto", ex);
            }
        }

        public async Task<ProdutoEntity> AlterarProduto(ProdutoEntity produto)
        {
            try
            {
                if (produto == null)
                {
                    throw new ArgumentException("Não foi possível alterar o produto.");
                }

                var produtoAntigo = (await ObterProdutoPeloId(produto.Id)).FirstOrDefault();

                if (produtoAntigo == null)
                {
                    throw new ArgumentException("Produto não encontrado para alteração!");
                }

                var produtoAlterado = await _repository.AlterarProduto(produto);
                return produtoAlterado;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao alterar produto", ex);
            }
        }

        public async Task<ProdutoEntity> ExcluirProduto(int idProduto)
        {
            try
            {
                if (idProduto == 0)
                {
                    throw new ArgumentException("ID do produto inválido.");
                }

                var produtoExcluido = await _repository.ExcluirProduto(idProduto);
                return produtoExcluido;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao excluir produto", ex);
            }
        }

        public async Task<List<ProdutoEntity>> ObterProdutos()
        {
            try
            {
                var produtos = await _repository.ObterProdutos();
                return produtos;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter produtos", ex);
            }
        }

        public async Task<List<ProdutoEntity>> ObterProdutoPeloCodigo(string codigo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codigo))
                {
                    throw new ArgumentException("Código do produto não pode ser nulo ou vazio.");
                }

                var produtos = await _repository.ObterProdutoPeloCodigo(codigo);
                return produtos;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter produto pelo código", ex);
            }
        }

        public async Task<List<ProdutoEntity>> ObterProdutoPeloId(int idProduto)
        {
            try
            {
                var produtos = await _repository.ObterProdutoPeloId(idProduto);
                return produtos;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter produto pelo ID", ex);
            }
        }
    }
}
