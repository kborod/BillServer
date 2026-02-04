using BilliardServer.Core.Models;

namespace BilliardServer.Core.Common
{
    public record AuthResult(
    bool IsSuccess,
    bool RequiresTwoFactor = false,
    bool IsLockedOut = false,
    string? Token = null,
    string? Error = null)
    {
        public static AuthResult Success(string token)
            => new(true, false, false, token);

        public static AuthResult Requires2FA(string token)
            => new(true, true, false, token);

        public static AuthResult Failure(string error)
            => new(false, false, false, null, error);

        public static AuthResult Lockout()
            => new(false, false, true, null, "Account locked");
    }
}
