using System.ComponentModel.DataAnnotations;

namespace TesteMVC.Models
{
    public class UsuarioViewModel
    {
        [Key]
        public int idUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public bool Ativo { get; set; }
    }

}
