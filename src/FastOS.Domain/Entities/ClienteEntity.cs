using System.ComponentModel.DataAnnotations;

namespace FastOS.Domain.Entities
{
    public class ClienteEntity
    {
        [Key]
        public int idCliente { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string? Endereco { get; set; }
        public bool Ativo { get; set; }
        public bool Excluido { get; set; }
        public int TipoCliente { get; set; }
        public string? CNPJ { get; set; }
        public string? CPF { get; set; }
    }
}
