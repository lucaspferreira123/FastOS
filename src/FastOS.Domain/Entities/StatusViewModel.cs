using System.ComponentModel.DataAnnotations;

namespace FastOS.Domain.Entities;

public class StatusViewModel
{
    [Key]
    public int idStatus { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
