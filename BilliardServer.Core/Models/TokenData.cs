namespace BilliardServer.Core.Models
{
    public class TokenData
    {
        public string? Token { get; set; }
        public long TokenExpiredTimestamp { get; set; }
        public string? RefreshToken { get; set; }
        public long RefreshTokenExpiredTimestamp { get; set; }
    }
}
