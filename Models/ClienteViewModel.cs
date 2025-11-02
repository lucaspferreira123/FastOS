using System.ComponentModel.DataAnnotations;

namespace TesteMVC.Models
{
    public class ClienteViewModel
    {
        [Key]
        public int idCliente { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Telefone { get; set; }
        public string? Endereco { get; set; }
    }
}
