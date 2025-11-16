using System.ComponentModel.DataAnnotations;

namespace TesteMVC.Models
{
    public class StatusViewModel
    {
        [Key]
        public int idStatus { get; set; }
        public string Descricao { get; set; }
    }
}
