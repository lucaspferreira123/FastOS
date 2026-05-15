using FastOS.Domain.Entities;
using FastOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastOS.Infrastructure.Repositories;

public class ProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<ProdutoEntity> GetAll()
    {
        return _context.Produto.ToList();
    }

    public async Task<ProdutoEntity> CadastrarProduto(ProdutoEntity produto)
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

    public async Task<List<ProdutoEntity>> ObterProdutos()
    {
        try
        {
            return await _context.Produto.ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao obter os produtos", ex);
        }
    }

    public async Task<List<ProdutoEntity>> ObterProdutoPeloNome(string nome)
    {
        try
        {
            return await _context.Produto
                .Where(p => p.NomeProduto == nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao obter produto pelo nome", ex);
        }
    }

    public async Task<List<ProdutoEntity>> ObterProdutoPeloId(int idProduto)
    {
        try
        {
            return await _context.Produto
                .Where(p => p.idProduto == idProduto)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao obter produto pelo id", ex);
        }
    }

    public async Task<ProdutoEntity> AlterarProduto(ProdutoEntity dadosAtualizados)
    {
        try
        {
            var produtoExistente = await _context.Produto
                .FirstOrDefaultAsync(p => p.idProduto == dadosAtualizados.idProduto);

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

    public async Task<ProdutoEntity> ExcluirProduto(int idProduto)
    {
        try
        {
            var produto = await _context.Produto.FirstOrDefaultAsync(p => p.idProduto == idProduto);
            if (produto == null)
                throw new Exception("Produto não encontrado.");

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
