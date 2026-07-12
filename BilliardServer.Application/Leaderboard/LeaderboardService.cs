using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Dto.Leaderboard;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace BilliardServer.Application.Leaderboard
{
    public class LeaderboardService : BackgroundService, ILeaderboardService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<LeaderboardService> _logger;
        private const string LeaderboardKey = "leaderboard:top";

        public LeaderboardService(
            IConnectionMultiplexer redis,
            ILogger<LeaderboardService> logger)
        {
            _redis = redis;
            _logger = logger;
        }

        public async Task<List<LeaderboardEntryDto>> GetTopLeaderboard(int limit = 100)
        {
            try
            {
                var db = _redis.GetDatabase();
                var cachedData = await db.StringGetAsync(LeaderboardKey);

                if (cachedData.HasValue)
                {
                    var leaderboard = JsonSerializer.Deserialize<List<LeaderboardEntryDto>>(
                        cachedData.ToString(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                        ?? new List<LeaderboardEntryDto>();

                    return leaderboard.Take(limit).ToList();
                }
                else
                {
                    return new List<LeaderboardEntryDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top leaderboard");
                return new List<LeaderboardEntryDto>();
            }
        }

        public async Task<int> GetUserRank(long userId)
        {
            try
            {
                var leaderboard = await GetTopLeaderboard(int.MaxValue);
                var userEntry = leaderboard.FirstOrDefault(entry => entry.UserId == userId);
                return userEntry?.Position ?? -1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user rank for userId {userId}");
                return -1;
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
