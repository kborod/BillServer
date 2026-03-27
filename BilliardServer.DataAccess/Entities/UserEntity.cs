using Microsoft.AspNetCore.Identity;

namespace BilliardServer.Infrastructure.Entities
{
    public class UserEntity : IdentityUser<long>
    {
        public string Name { get; set; } = string.Empty;
        public int Avatar { get; set; }

        public int Exp { get; set; }
        public int Rating { get; set; }

        public int Chips { get; set; }
        public int Coins { get; set; }

        public int PartiesCount { get; set; }
        public int WinPartiesCount { get; set; }
        public int TotalChipsPrize { get; set; }
    }
}
