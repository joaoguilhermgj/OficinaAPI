using Microsoft.AspNetCore.Identity;

namespace Oficina.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; } = string.Empty;
    }
}
