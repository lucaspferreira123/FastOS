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
        public string DescricaoServico { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime PrevisaoEntrega { get; set; }
        public ICollection<ItemOrdemServicoViewModel> Itens { get; set; }
    }
}
