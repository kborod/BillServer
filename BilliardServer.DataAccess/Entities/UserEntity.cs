using Microsoft.AspNetCore.Identity;

namespace BilliardServer.Infrastructure.Entities
{
    public class UserEntity : IdentityUser<long>
    {
        public string Name { get; set; } = string.Empty;
        public int Avatar { get; set; }
    }
}
