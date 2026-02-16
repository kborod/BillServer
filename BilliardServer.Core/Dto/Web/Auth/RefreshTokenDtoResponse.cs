using BilliardServer.Core.Models;

namespace BilliardServer.Core.Common.Dto.Auth
{
    public class RefreshTokenDtoResponse
    {
        public TokenData? TokenData { get; set; }
    }
}