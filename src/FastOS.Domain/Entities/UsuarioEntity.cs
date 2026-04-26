using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastOS.Domain.Entities;

public class UsuarioEntity
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "varchar(200)")]
    [MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    [Column(TypeName = "varchar(200)")]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Column(TypeName = "varchar(max)")]
    public string Senha { get; set; } = string.Empty;

    public bool Ativo { get; set; }
    public bool Excluido { get; set; }

    [Column(TypeName = "varchar(100)")]
    [MaxLength(100)]
    public string? Cargo { get; set; }
}
