using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TesteMVC.Dto;
using TesteMVC.Dto.TesteMVC.Dto;
using TesteMVC.IRepository;
using TesteMVC.Models;
using TesteMVC.Repository;

namespace MeuProjeto.Repository
{
    public class ItemOrdemServicoRepository : BaseRepository<ItemOrdemServicoViewModel>, IItemOrdemServicoRepository
    {
        private readonly AppDbContext _context;

        public ItemOrdemServicoRepository(AppDbContext context): base(context)
        {
            _context = context;
        }

        public async Task<List<ItemOrdemServicoViewModel>> AlterarItensOrdemServico(List<ItemOrdemServicoViewModel> itens)
        {
            try
            {
                int idOS = itens.First().idOrdemServico;

                // Itens atuais no banco
                var itensBanco = await _context.ItensOrdemServico
                    .Where(i => i.idOrdemServico == idOS)
                    .ToListAsync();

                foreach (var item in itens)
                {
                    var existente = itensBanco
                        .FirstOrDefault(i => i.idProduto == item.idProduto);

                    if (existente == null)
                    {
                        // INSERIR
                        item.DataPedido = DateTime.Now;
                        _context.ItensOrdemServico.Add(item);
                    }
                    else
                    {
                        // ATUALIZAR
                        existente.Quantidade = item.Quantidade;
                        existente.DataRealizado = item.DataRealizado;
                    }
                }

                // EXCLUIR OS QUE NÃO ESTÃO MAIS NA LISTA
                var idsProdutosEnviados = itens.Select(i => i.idProduto).ToList();

                var itensRemover = itensBanco
                    .Where(i => !idsProdutosEnviados.Contains(i.idProduto))
                    .ToList();

                _context.ItensOrdemServico.RemoveRange(itensRemover);

                await _context.SaveChangesAsync();

                return itens;
            }
            catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message;
                throw new Exception($"Erro ao salvar os itens da ordem: {innerMessage}", ex);
            }
        }


        public async Task<List<ItensOrdemServicoDto>> ObterItensOrdemServico(int idOrdem)
        {
            var itens = await _context.ItensOrdemServico
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
                }
            ).Where(i => i.idOrdemServico == idOrdem)
            .ToListAsync();

            return itens;
        }
    }
}
