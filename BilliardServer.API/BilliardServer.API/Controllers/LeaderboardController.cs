using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Dto.Leaderboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BilliardServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;
        private readonly ILogger<LeaderboardController> _logger;

        public LeaderboardController(ILeaderboardService leaderboardService, ILogger<LeaderboardController> logger)
        {
            _leaderboardService = leaderboardService;
            _logger = logger;
        }

        /// <summary>
        /// Получить топ таблицу лидеров
        /// </summary>
        /// <param name="limit">Количество записей для возврата (по умолчанию 100)</param>
        /// <returns>Список лидеров</returns>
        [HttpGet("top")]
        [AllowAnonymous]
        public async Task<ActionResult<List<LeaderboardEntryDto>>> GetTopLeaderboard([FromQuery] int limit = 100)
        {
            try
            {
                if (limit <= 0 || limit > 1000)
                    limit = 100;

                var leaderboard = await _leaderboardService.GetTopLeaderboard(limit);
                return Ok(leaderboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top leaderboard");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получить позицию пользователя в таблице лидеров
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Позиция в таблице или -1 если не найден</returns>
        [HttpGet("rank/{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<int>> GetUserRank(long userId)
        {
            try
            {
                var rank = await _leaderboardService.GetUserRank(userId);
                return Ok(new { rank });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting rank for user {userId}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
