using BilliardServer.Core.Common;
using Kborod.BilliardCore.Enums;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace BilliardServer.Application.Matches
{
    public class MatchesService : BackgroundService
    {
        private readonly TimeSpan _checkMatchesPeriod = TimeSpan.FromSeconds(3);
        private ConcurrentDictionary<string, Match> _matches = new();

        private readonly ILogger _logger;

        private int _lastMatchId = 0;

        private int _newMatchId => Interlocked.Increment(ref _lastMatchId);

        public MatchesService(ILogger logger)
        {
            _logger = logger;
        }

        public Task<Result> CreateMatch(string player1, string player2, GameType gameType, BetType betType)
        {
            var id = _newMatchId.ToString();
            var match = new Match(id, player1, player2);
            _matches.TryAdd(id, match);
            _logger.LogInformation($"Created match {id} for players {player1} and {player2}");
            return Task.FromResult(Result.Ok());
        }

        public Task DeleteMatch(string matchId)
        {
            _matches.TryRemove(matchId, out var _);
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var pair in _matches)
                {
                    pair.Value.PeriodicCheck();
                }

                await Task.Delay(_checkMatchesPeriod, stoppingToken);
            }
        }
    }
}
