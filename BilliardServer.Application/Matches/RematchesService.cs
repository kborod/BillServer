using BilliardServer.Application.Abstractions.AsyncMessaging;
using BilliardServer.Application.Features.Matches;
using BilliardServer.Application.Features.Users;
using BilliardServer.Core.Common;
using BilliardServer.Core.Dto.Messaging.Responses;
using BilliardServer.Core.Dto.Messaging.Responses.Match;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace BilliardServer.Application.Matches
{
    public class RematchesService : BackgroundService
    {
        private readonly IMediator _mediator;
        private readonly IMessagingResponseSenderService _responseSender;
        private readonly ILogger _logger;

        private readonly ConcurrentDictionary<string, RematchContext> _rematches = new();
        private readonly ConcurrentDictionary<string, string> _userToRematchId = new();

        private readonly TimeSpan _checkMatchesPeriod;
        private readonly int _waitReplaySeconds;

        public RematchesService(IOptions<MatchesServiceConfig> _config, IMediator mediator, IMessagingResponseSenderService responseSender, ILogger logger)
        {
            _mediator = mediator;
            _responseSender = responseSender;
            _logger = logger;
            
            _checkMatchesPeriod = TimeSpan.FromSeconds(_config.Value.CheckMatchesPeriodSeconds);
            _waitReplaySeconds = _config.Value.WaitReplayMatchSeconds;
            
            _logger.LogInformation($"[MatchReplayService] Initialized with check period {_checkMatchesPeriod.TotalSeconds} seconds");
        }

        public Task<Result> AddFinishedMatch(MatchContext context)
        {
            var rematchContext = new RematchContext(
                context.Player1,
                context.Player2,
                context.TurningPlayer == context.Player1 ? context.Player2 : context.Player1,
                context.GameType,
                context.BetType,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds() + _waitReplaySeconds);

            _rematches.TryAdd(context.Id, rematchContext);
            _userToRematchId.TryAdd(context.Player1, context.Id);
            _userToRematchId.TryAdd(context.Player2, context.Id);

            return Task.FromResult(Result.Ok());
        }

        public Task UserDisconnected(string userId)
        { 
            if (_userToRematchId.TryGetValue(userId, out var matchId))
                UserCancelRematch(userId, matchId);

            return Task.CompletedTask;
        }

        public async Task<Result> UserReadyRematch(string userId, string matchId)
        {
            if (_rematches.TryGetValue(matchId, out var context) == false)
            {
                _ = _responseSender.SendResponseToUser(userId, new OppCancelRematchResponseDto(matchId));
                return Result.Ok();
            }

            var isOppHaveActiveConnection = await _mediator.Send(new IsUserHaveActiveConnectionCommand(context.GetOpponent(userId)));

            lock (context)
            {
                if (context.IsOppReady(userId) && isOppHaveActiveConnection)
                {
                    _ = _mediator.Send(new CreateMatchCommand(context.Player1, context.Player2, context.TurningPlayer, context.GameType, context.BetType));
                    DeleteRematch(matchId);
                }
                else
                {
                    context.SetPlayerReady(userId);
                    _responseSender.SendResponseToUser(userId, new ConfirmResponseDto("Waiting opponent for replay match"));
                    _responseSender.SendResponseToUser(context.GetOpponent(userId), new OppReadyRematchResponseDto(matchId));
                }
            }

            return Result.Ok();
        }

        public Result UserCancelRematch(string userId, string matchId)
        {
            if (_rematches.TryGetValue(matchId, out var context) == false)
            {
                return Result.Ok();
            }

            _responseSender.SendResponseToUser(userId, new CancelRematchConfirmResponseDto(matchId));
            _responseSender.SendResponseToUser(context.GetOpponent(userId), new OppCancelRematchResponseDto(matchId));
            DeleteRematch(matchId);

            return Result.Ok();
        }

        private void DeleteRematch(string matchId)
        {
            if (_rematches.TryRemove(matchId, out var context))
            {
                _userToRematchId.TryRemove(context.Player1, out var _);
                _userToRematchId.TryRemove(context.Player2, out var _);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var pairs = _rematches.ToArray();

                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                
                foreach (var pair in pairs)
                {
                    if (!_rematches.ContainsKey(pair.Key))
                        continue;

                    if (pair.Value.EndWaitTimestamp < timestamp)
                    {
                        DeleteRematch(pair.Key);
                    }
                }

                await Task.Delay(_checkMatchesPeriod, stoppingToken);
            }
        }
    }
}
