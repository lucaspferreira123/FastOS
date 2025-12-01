using TesteMVC.Dto.TesteMVC.Dto;
using TesteMVC.Models;

namespace TesteMVC.Dto
{
    public class RelatorioOrcamentoDto
    {
        public OrdemServicoDto OrdemServico { get; set; }
        public OrcamentoViewModel Orcamento { get; set; }
        public List<ItensOrdemServicoDto> Itens { get; set; }
    }
}
