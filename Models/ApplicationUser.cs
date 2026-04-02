using Microsoft.AspNetCore.Identity;

namespace TesteMVC.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string Nome { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
    }
}
