using BilliardServer.Application.Features.Users;
using BilliardServer.Core.Common;
using Kborod.BilliardCore.Enums;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace BilliardServer.Application.Matches
{
    public class MatchesService : BackgroundService
    {
        private readonly ConcurrentDictionary<string, MatchControl> _matches = new();
        private readonly ConcurrentDictionary<string, MatchControl> _users = new();

        private readonly TimeSpan _checkMatchesPeriod;
        private readonly ILogger _logger;
        private readonly IMatchControlFactory _matchControlFactory;
        private readonly IServiceProvider _serviceProvider;

        private int _lastMatchId = 0;

        private int _newMatchId => Interlocked.Increment(ref _lastMatchId);

        public MatchesService(IOptions<MatchesServiceConfig> _config, ILogger logger, IServiceProvider sp, 
            IMatchControlFactory matchControlFactory, IServiceProvider serviceProvider)
        {
            _checkMatchesPeriod = TimeSpan.FromSeconds(_config.Value.CheckMatchesPeriodSeconds);
            _logger = logger;
            _matchControlFactory = matchControlFactory;
            _serviceProvider = serviceProvider;

            _logger.LogInformation($"[MatchesService] Initialized with check period {_checkMatchesPeriod.TotalSeconds} seconds");
        }

        public async Task<Result> CreateMatch(string player1, string player2, GameType gameType, BetType betType)
        {
            var id = _newMatchId.ToString();
            var posNum = 1;

            var context = new CreateMatchContext(id, player1, player2, player1, gameType, betType, posNum);
            
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var profilesResult = await mediator.Send(new GetUserProfilesCommand(new() { player1, player2 }));
            var profiles = profilesResult.Value!;

            var match = _matchControlFactory.Create(context);
            var initResult = await match.Init(profiles);

            if (initResult.IsSuccess == false)
                return initResult;

            _matches.TryAdd(id, match);
            _users.TryAdd(player1, match);
            _users.TryAdd(player2, match);

            _logger.LogInformation($"[MatchesService] Created match {id} for players {player1} and {player2}");
            return Result.Ok();
        }

        public Task DeleteMatch(string matchId)
        {
            if (_matches.TryRemove(matchId, out var match))
            {
                _users.TryRemove(match.Context.Player1, out _);
                _users.TryRemove(match.Context.Player2, out _);
            }
            
            return Task.CompletedTask;
        }

        public Task UserDisconnected(string userId)
        { 
            if (_users.TryGetValue(userId, out var match))
            {
                match.UserDisconnected(userId);
            }
            return Task.CompletedTask;
        }

        public bool IsUserInMatch(string userId)
        {
            return _users.ContainsKey(userId);
        }

        public Task<MatchControl?> GetMatch(string matchId)
        {
            _matches.TryGetValue(matchId, out var match);
            return Task.FromResult(match);
        }

        public Task<MatchControl?> GetMatchByUser(string userId)
        {
            _users.TryGetValue(userId, out var match);
            return Task.FromResult(match);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                foreach (var pair in _matches)
                {
                    pair.Value.PeriodicCheck(timestamp);
                }

                await Task.Delay(_checkMatchesPeriod, stoppingToken);
            }
        }
    }
}
