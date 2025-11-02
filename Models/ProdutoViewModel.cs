using System.ComponentModel.DataAnnotations;

namespace TesteMVC.Models
{
    public class ProdutoViewModel
    {
        [Key]
        public int idProduto { get; set; }
        public string NomeProduto { get; set; }
        public string Descricao { get; set; }
        public decimal PrecoUnitario { get; set; }
        public int QuantidadeTotal { get; set; }
        public string Marca { get; set; }
    }
}
