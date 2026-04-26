using BilliardServer.Core.Common;
using BilliardServer.Core.Models;
using Kborod.SharedDto;

namespace BilliardServer.Core.Abstractions
{
    public interface IUsersService
    {
        Task<Result<User?>> GetUser(string id);
        Task<Result<User?>> GetByEmail(string email);
        Task<Result<UserProfile>> GetUserProfile(string id);
        Task<Result<List<UserProfile>>> GetUserProfiles(List<string> ids);
        Task<Result> SetAvatar(string userId, int avatarId);
    }
}