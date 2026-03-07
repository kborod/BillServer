using BilliardServer.Core.Common;
using BilliardServer.Core.Models;
using Kborod.SharedDto;

namespace BilliardServer.Core.Abstractions
{
    public interface IUsersService
    {
        Task<Result<User?>> GetUser(string id);
        Task<Result<User?>> GetByEmail(string email);
        Task<Result<UserProfileDto>> GetUserProfile(string id);
        Task<Result<List<UserProfileDto>>> GetUserProfiles(List<string> ids);
    }
}