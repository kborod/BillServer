using BilliardServer.Application.Abstractions.AsyncMessaging;
using BilliardServer.Application.Matches;
using BilliardServer.Core.Common;
using BilliardServer.Core.Dto.Messaging.Responses.MatchMaking;
using Kborod.BilliardCore.Enums;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;

namespace BilliardServer.Application.MatchMaking
{
    public class MatchMakingService : BackgroundService
    {
        private IMessagingResponseSenderService _messagingResponseSenderService;
        private MatchesService _matchesRepository;

        private ConcurrentDictionary<(GameType, BetType), ConcurrentDictionary<string, long>> _queues = new();
        private ConcurrentDictionary<string, (GameType, BetType)> _users = new();

        public MatchMakingService(IMessagingResponseSenderService messagingResponseSenderService, MatchesService matchesRepository)
        {
            _messagingResponseSenderService = messagingResponseSenderService;
            _matchesRepository = matchesRepository;
        }

        public Task<Result> SearchMatch(string userId, GameType gameType, BetType betType)
        {
            if (_users.ContainsKey(userId))
                return Task.FromResult(Result.Fail($"User {userId} already waiting"));

            var waitingUsers = _queues.GetOrAdd((gameType, betType), _ => new ConcurrentDictionary<string, long>());

            string? opponentId = null;
            while(waitingUsers.Count > 0)
            {
                var pair = waitingUsers.MinBy(pair => pair.Value);
                if (waitingUsers.TryRemove(pair))
                {
                    opponentId = pair.Key;
                    break;
                }
            }
            if (opponentId != null)
            {
                _users.TryRemove(opponentId, out var _);
                return _matchesRepository.CreateMatch(userId, opponentId, gameType, betType);
            }
            else
            {
                _users.TryAdd(userId, (gameType, betType));
                waitingUsers.TryAdd(userId, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                _messagingResponseSenderService.SendResponseToUser(userId, new AddedToQueueResponseDto());
            }

            return Task.FromResult(Result.Ok());
        }

        public Task CancelSearch(string userId, bool withNotice = false)
        {
            if (_users.TryRemove(userId, out var gameAndBet))
            {
                var queue = _queues.GetOrAdd(gameAndBet, _ => new ConcurrentDictionary<string, long>());
                queue.TryRemove(userId, out var _);

                if (withNotice)
                    _messagingResponseSenderService.SendResponseToUser(userId, new SearchCancelledResponseDto());
            }

            return Task.CompletedTask;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
