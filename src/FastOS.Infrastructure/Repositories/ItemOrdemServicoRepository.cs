using FastOS.Domain.Entities;
using FastOS.Domain.Interfaces;
using FastOS.Domain.ValueObjects;
using FastOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastOS.Infrastructure.Repositories;

public class ItemOrdemServicoRepository : BaseRepository<ItemOrdemServicoEntity>, IItemOrdemServicoRepository
{
    private readonly AppDbContext _context;

    public ItemOrdemServicoRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<ItemOrdemServicoEntity>> AlterarItensOrdemServico(List<ItemOrdemServicoEntity> itens)
    {
        try
        {
            var idOS = itens.First().idOrdemServico;
            var itensBanco = await _context.ItensOrdemServico
                .Where(i => i.idOrdemServico == idOS)
                .ToListAsync();

            var qtdAnterior = itensBanco.ToDictionary(i => i.idProduto, i => i.Quantidade);
            var qtdNova = itens.ToDictionary(i => i.idProduto, i => i.Quantidade);

            var removidos = itensBanco.Where(i => !qtdNova.ContainsKey(i.idProduto)).ToList();
            foreach (var removido in removidos)
            {
                var produto = await _context.Produto.FindAsync(removido.idProduto);
                if (produto != null)
                {
                    produto.Quantidade += removido.Quantidade;
                }
            }

            foreach (var item in itens)
            {
                var produto = await _context.Produto.FindAsync(item.idProduto);
                if (produto == null)
                {
                    continue;
                }

                if (qtdAnterior.TryGetValue(item.idProduto, out var quantidadeAnterior))
                {
                    var diferenca = item.Quantidade - quantidadeAnterior;
                    if (diferenca > 0 && produto.Quantidade < diferenca)
                    {
                        throw new InvalidOperationException(
                            $"Estoque insuficiente para \"{produto.Descricao}\". " +
                            $"Quantidade disponivel: {produto.Quantidade} unidade(s). " +
                            $"Quantidade adicional solicitada: {diferenca} unidade(s).");
                    }

                    produto.Quantidade -= diferenca;
                }
                else
                {
                    if (produto.Quantidade < item.Quantidade)
                    {
                        throw new InvalidOperationException(
                            $"Estoque insuficiente para \"{produto.Descricao}\". " +
                            $"Quantidade disponivel: {produto.Quantidade} unidade(s). " +
                            $"Quantidade solicitada: {item.Quantidade} unidade(s).");
                    }

                    produto.Quantidade -= item.Quantidade;
                }
            }

            foreach (var item in itens)
            {
                var existente = itensBanco.FirstOrDefault(i => i.idProduto == item.idProduto);
                if (existente == null)
                {
                    item.DataPedido = DateTime.Now;
                    _context.ItensOrdemServico.Add(item);
                }
                else
                {
                    existente.Quantidade = item.Quantidade;
                    existente.DataRealizado = item.DataRealizado;
                }
            }

            var idsProdutosEnviados = itens.Select(i => i.idProduto).ToList();
            var itensRemover = itensBanco.Where(i => !idsProdutosEnviados.Contains(i.idProduto)).ToList();
            _context.ItensOrdemServico.RemoveRange(itensRemover);

            await _context.SaveChangesAsync();
            return itens;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (DbUpdateException ex)
        {
            var innerMessage = ex.InnerException?.Message;
            throw new Exception($"Erro ao salvar os itens da ordem: {innerMessage}", ex);
        }
    }

    public async Task<List<ItensOrdemServicoDto>> ObterItensOrdemServico(int idOrdem)
    {
        return await _context.ItensOrdemServico
            .Join(
                _context.Produto.Where(p => !p.Excluido),
                item => item.idProduto,
                prod => prod.Id,
                (item, prod) => new ItensOrdemServicoDto
                {
                    idOrdemServico = item.idOrdemServico,
                    idItemOrdemServico = item.idItemOrdem,
                    idProduto = prod.Id,
                    nomeProduto = $"{prod.Codigo} - {prod.Descricao}",
                    quantidade = item.Quantidade.ToString(),
                    DescricaoServico = prod.Descricao,
                    valorUnitario = 0
                })
            .Where(i => i.idOrdemServico == idOrdem)
            .ToListAsync();
    }
}
