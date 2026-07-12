using BilliardServer.Core.Dto.Leaderboard;

namespace BilliardServer.Core.Abstractions
{
    public interface ILeaderboardService
    {
        /// <summary>
        /// Получить топ лидеров
        /// </summary>
        /// <param name="limit">Количество записей для возврата</param>
        /// <returns>Список лидеров</returns>
        Task<List<LeaderboardEntryDto>> GetTopLeaderboard(int limit = 100);

        /// <summary>
        /// Получить рейтинг пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Позиция в таблице лидеров или -1 если пользователь не найден</returns>
        Task<int> GetUserRank(long userId);
    }
}
