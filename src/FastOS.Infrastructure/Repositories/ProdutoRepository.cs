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
        return _context.Produto
            .Where(p => !p.Excluido)
            .ToList();
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

    public async Task<List<ProdutoEntity>> ObterProdutoPeloCodigo(string codigo)
    {
        try
        {
            return await _context.Produto
                .Where(p => !p.Excluido && p.Codigo == codigo)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao obter produto pelo codigo", ex);
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
                .FirstOrDefaultAsync(p => p.Id == dadosAtualizados.Id && !p.Excluido);

            if (produtoExistente == null)
                throw new Exception("Produto não encontrado.");

            produtoExistente.Descricao = dadosAtualizados.Descricao;
            produtoExistente.Codigo = dadosAtualizados.Codigo;
            produtoExistente.ValorCusto = dadosAtualizados.ValorCusto;
            produtoExistente.ValorVenda = dadosAtualizados.ValorVenda;
            produtoExistente.Quantidade = dadosAtualizados.Quantidade;
            produtoExistente.QuantidadeMinimaEstoque = dadosAtualizados.QuantidadeMinimaEstoque;

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
            var produto = await _context.Produto.FirstOrDefaultAsync(p => p.Id == idProduto && !p.Excluido);
            if (produto == null)
                throw new Exception("Produto não encontrado.");

            produto.Excluido = true;
            _context.Produto.Update(produto);
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
