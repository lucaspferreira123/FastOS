using Microsoft.AspNetCore.Identity;

namespace FastOS.Domain.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}
