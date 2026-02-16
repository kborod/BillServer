using BilliardServer.Core.Models;

namespace BilliardServer.Core.Common
{
    public record AuthByProviderResult(
    bool IsSuccess,
    bool IsFirstAuth = false,
    TokenData? Token = null,
    User? User = null,
    string? Error = null)
    {
        public static AuthByProviderResult Ok(TokenData token, User user, bool isFirstAuth)
            => new(true, isFirstAuth, token, user);

        public static AuthByProviderResult Fail(string error)
            => new(false, Error: error);
    }
}
