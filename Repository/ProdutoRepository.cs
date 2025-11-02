using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TesteMVC.Models;

namespace MeuProjeto.Repository
{
    public class ProdutoRepository
    {
        private readonly AppDbContext _context;

        public ProdutoRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<ProdutoViewModel> GetAll()
        {
            return _context.Produto.ToList();
        }

        public async Task<ProdutoViewModel> CadastrarProduto(ProdutoViewModel produto)
        {
            try
            {
                _context.Produto.Add(produto);
                await _context.SaveChangesAsync();

                return produto;
            }
            catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message;
                throw new Exception($"Erro ao salvar produto: {innerMessage}", ex);
            }
        }

        public async Task<List<ProdutoViewModel>> ObterProdutos()
        {
            try
            {
                var produtos = await _context.Produto.FromSqlRaw(@"SELECT * FROM Produto").ToListAsync();
                return produtos;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter os produtos", ex);
            }
        }

        public async Task<List<ProdutoViewModel>> ObterProdutoPeloNome(string nome)
        {
            try
            {
                var param = new SqlParameter("@nome", nome);

                var produtos = await _context.Produto
                    .FromSqlRaw("SELECT * FROM Produto WHERE NomeProduto = @nome", param)
                    .ToListAsync();

                return produtos;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter produto pelo nome", ex);
            }
        }

        public async Task<List<ProdutoViewModel>> ObterProdutoPeloId(int idProduto)
        {
            try
            {
                var param = new SqlParameter("@idProduto", idProduto);

                var produtos = await _context.Produto
                    .FromSqlRaw("SELECT * FROM Produto WHERE idProduto = @idProduto", param)
                    .ToListAsync();

                return produtos;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter produto pelo id", ex);
            }
        }

        public async Task<ProdutoViewModel> AlterarProduto(ProdutoViewModel dadosAtualizados)
        {
            try
            {
                var produtoExistente = await _context.Produto
                    .FromSqlRaw("SELECT * FROM Produto WHERE idProduto = @idProduto",
                        new SqlParameter("@idProduto", dadosAtualizados.idProduto))
                    .FirstOrDefaultAsync();

                if (produtoExistente == null)
                    throw new Exception("Produto não encontrado.");

                produtoExistente.NomeProduto = dadosAtualizados.NomeProduto;
                produtoExistente.Descricao = dadosAtualizados.Descricao;
                produtoExistente.PrecoUnitario = dadosAtualizados.PrecoUnitario;
                produtoExistente.QuantidadeTotal = dadosAtualizados.QuantidadeTotal;
                produtoExistente.Marca = dadosAtualizados.Marca;

                _context.Produto.Update(produtoExistente);
                await _context.SaveChangesAsync();

                return produtoExistente;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar produto", ex);
            }
        }

        public async Task<ProdutoViewModel> ExcluirProduto(int idProduto)
        {
            try
            {
                var produto = await _context.Produto.FirstOrDefaultAsync(p => p.idProduto == idProduto);

                if (produto == null)
                {
                    throw new Exception("Produto não encontrado.");
                }

                _context.Produto.Remove(produto);
                await _context.SaveChangesAsync();

                return produto;
            }
            catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message;
                throw new Exception($"Erro ao excluir produto: {innerMessage}", ex);
            }
        }
    }
}
