using BilliardServer.Core.Common;
using BilliardServer.Core.Models;

namespace BilliardServer.Core.Abstractions
{
    public interface IUsersRepository
    {
        Task<Result<User?>> GetUser(string id);
        Task<Result<User?>> GetByEmail(string email);
        Task Delete(string id);
        Task Update(string id, string name, int avatar);
    }
}