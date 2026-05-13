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
            var itensBanco = await _context.ItensOrdemServico.Where(i => i.idOrdemServico == idOS).ToListAsync();

            // ── Controle de estoque ──────────────────────────────────────
            // Monta dicionário de quantidades anteriores por produto
            var qtdAnterior = itensBanco.ToDictionary(i => i.idProduto, i => i.Quantidade);
            var qtdNova     = itens.ToDictionary(i => i.idProduto, i => i.Quantidade);

            // Produtos que serão removidos → devolver ao estoque
            var removidos = itensBanco.Where(i => !qtdNova.ContainsKey(i.idProduto)).ToList();
            foreach (var r in removidos)
            {
                var produto = await _context.Produto.FindAsync(r.idProduto);
                if (produto != null)
                    produto.QuantidadeTotal += r.Quantidade;
            }

            // Produtos novos ou com quantidade alterada
            foreach (var item in itens)
            {
                var produto = await _context.Produto.FindAsync(item.idProduto);
                if (produto == null) continue;

                if (qtdAnterior.TryGetValue(item.idProduto, out var anterior))
                {
                    // Já existia — ajusta diferença
                    var diferenca = item.Quantidade - anterior;
                    if (diferenca > 0 && produto.QuantidadeTotal < diferenca)
                        throw new InvalidOperationException(
                            $"Estoque insuficiente para \"{produto.NomeProduto}\". " +
                            $"Quantidade disponível: {produto.QuantidadeTotal} unidade(s). " +
                            $"Quantidade adicional solicitada: {diferenca} unidade(s).");
                    produto.QuantidadeTotal -= diferenca;
                }
                else
                {
                    // Item novo — desconta tudo
                    if (produto.QuantidadeTotal < item.Quantidade)
                        throw new InvalidOperationException(
                            $"Estoque insuficiente para \"{produto.NomeProduto}\". " +
                            $"Quantidade disponível: {produto.QuantidadeTotal} unidade(s). " +
                            $"Quantidade solicitada: {item.Quantidade} unidade(s).");
                    produto.QuantidadeTotal -= item.Quantidade;
                }
            }

            // ── Persiste itens ───────────────────────────────────────────
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
                _context.Produto,
                item => item.idProduto,
                prod => prod.idProduto,
                (item, prod) => new ItensOrdemServicoDto
                {
                    idOrdemServico = item.idOrdemServico,
                    idItemOrdemServico = item.idItemOrdem,
                    idProduto = prod.idProduto,
                    nomeProduto = prod.NomeProduto,
                    quantidade = item.Quantidade.ToString(),
                    DescricaoServico = prod.Descricao,
                    valorUnitario = prod.PrecoUnitario
                })
            .Where(i => i.idOrdemServico == idOrdem)
            .ToListAsync();
    }
}
