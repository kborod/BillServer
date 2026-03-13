using BilliardServer.Application.Abstractions.AsyncMessaging;
using BilliardServer.Application.Features.Matches;
using BilliardServer.Application.Features.MatchMaking;
using BilliardServer.Application.Features.Users;
using BilliardServer.Core.Common;
using BilliardServer.Core.Dto.Messaging.Responses.MatchMaking;
using Kborod.BilliardCore.Enums;
using MediatR;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;

namespace BilliardServer.Application.MatchMaking
{
    public class MatchMakingService : BackgroundService
    {
        private readonly IMessagingResponseSenderService _messagingResponseSenderService;
        private readonly IMediator _mediator;

        private readonly ConcurrentDictionary<(GameType, BetType), ConcurrentDictionary<string, long>> _queues = new();
        private readonly ConcurrentDictionary<string, (GameType, BetType)> _users = new();

        public MatchMakingService(IMessagingResponseSenderService messagingResponseSenderService, IMediator mediator)
        {
            _messagingResponseSenderService = messagingResponseSenderService;
            _mediator = mediator;
        }

        public async Task<Result> SearchMatch(string userId, GameType gameType, BetType betType)
        {
            if (_users.ContainsKey(userId))
                return Result.Fail($"User {userId} already waiting");

            var isInMatch = await _mediator.Send(new IsUserInMatchCommand(userId));
            if (isInMatch)
                return Result.Fail($"User {userId} already in match");

            var waitingUsers = _queues.GetOrAdd((gameType, betType), _ => new ConcurrentDictionary<string, long>());

            string? opponentId = null;

            if (waitingUsers.Count > 0)
            {
                var usersWithActiveConnection = await _mediator.Send(new GetUsersWithActiveConnectionCommand(waitingUsers.Keys));

                while (usersWithActiveConnection.Count > 0)
                {
                    var maxWaitingUser = usersWithActiveConnection.MinBy(u =>
                    {
                        if (waitingUsers.TryGetValue(u, out var enterQueueTime))
                            return enterQueueTime;
                        return long.MaxValue;
                    });
                    if (waitingUsers.TryRemove(maxWaitingUser!, out var _))
                    {
                        opponentId = maxWaitingUser;
                        break;
                    }
                    else
                    {
                        usersWithActiveConnection.Remove(maxWaitingUser!);
                    }
                }
            }
            
            if (opponentId != null)
            {
                _users.TryRemove(opponentId, out var _);
                var turningPlayer = Random.Shared.Next(2) > 0 ? userId : opponentId;
                return await _mediator.Send(new CreateMatchCommand(userId, opponentId, turningPlayer, gameType, betType));
            }
            else
            {
                _users.TryAdd(userId, (gameType, betType));
                waitingUsers.TryAdd(userId, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                await _messagingResponseSenderService.SendResponseToUser(userId, new AddedToQueueResponseDto());
            }

            return Result.Ok();
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
