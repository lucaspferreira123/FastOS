using System.ComponentModel.DataAnnotations;

namespace FastOS.Domain.Entities;

public class UsuarioEntity
{
    [Key]
    public int idUsuario { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}
