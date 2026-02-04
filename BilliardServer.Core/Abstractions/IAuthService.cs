using BilliardServer.Core.Common;
using BilliardServer.Core.Models;

namespace BilliardServer.Core.Abstractions
{
    public interface IAuthService
    {
        Task<Result<User>> RegisterByEmail(string name, string email, string password);
        Task<AuthResult> LoginByEmail(string email, string password);
        Task<AuthByProviderResult> LoginByProvider(string provider, string providerKey, string name, string? email);
    }
}