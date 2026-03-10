using Microsoft.AspNetCore.Identity;

namespace LojaDoImovel.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpireTime { get; set; }
}
