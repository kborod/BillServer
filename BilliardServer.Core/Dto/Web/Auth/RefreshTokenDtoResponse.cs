using BilliardServer.Core.Models;

namespace BilliardServer.Core.Dto.Web.Auth
{
    public class RefreshTokenDtoResponse
    {
        public TokenData? TokenData { get; set; }
    }
}