using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Dto.Leaderboard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace BilliardServer.Application.Leaderboard
{
    public class LeaderboardInitializer : IHostedService
    {
        private const string LeaderboardKey = "leaderboard:top";

        private readonly IServiceProvider _serviceProvider;

        public LeaderboardInitializer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var usersRepository = scope.ServiceProvider.GetRequiredService<IUsersRepository>();
            var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger>();

            var db = redis.GetDatabase();

            try
            {
                logger.LogInformation("Refreshing leaderboard...");

                var users = await usersRepository.GetAllUsersForLeaderboard();

                var leaderboard = users
                    .OrderByDescending(u => u.Rating)
                    .ThenByDescending(u => u.WinPartiesCount)
                    .Select((user, index) => new LeaderboardEntryDto
                    {
                        UserId = long.Parse(user.Id),
                        UserName = user.Name,
                        Avatar = user.Avatar,
                        Position = index + 1
                    })
                    .ToList();

                var json = JsonSerializer.Serialize(leaderboard);
                await db.StringSetAsync(LeaderboardKey, json);

                logger.LogInformation($"Leaderboard refreshed successfully with {leaderboard.Count} entries");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error refreshing leaderboard");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
