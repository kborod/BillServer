using BilliardServer.Core.Common;
using BilliardServer.Core.Models;
using Kborod.SharedDto;

namespace BilliardServer.Core.Abstractions
{
    public interface IUsersRepository
    {
        Task<Result<User?>> GetUser(string id);
        Task<Result<User?>> GetByEmail(string email);
        Task<Result<UserProfile>> GetUserProfile(string id);
        Task<Result<List<UserProfile>>> GetUserProfiles(List<string> ids);

        Task<Result> UpdateAvatar(string userId, int avatarId);
        Task<Result> UpdateAfterMatch(string userId, int expDelta, int ratingDelta, bool isWin, int chipsPrize, int matchesCountDelta = +1);

        /// <summary>
        /// Получить всех пользователей для таблицы лидеров
        /// </summary>
        Task<List<User>> GetAllUsersForLeaderboard();
    }
}