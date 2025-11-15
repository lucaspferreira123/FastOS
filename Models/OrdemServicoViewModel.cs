using System.ComponentModel.DataAnnotations;

namespace TesteMVC.Models
{
    public class OrdemServicoViewModel
    {
        [Key]
        public int idOrdemServico { get; set; }
        public int idCliente { get; set; }
        public bool Pago { get; set; }
        public int idStatus { get; set; }
        public int DescricaoServico { get; set; }
        public ICollection<ItemOrdemServicoViewModel> Itens { get; set; }
    }
}
