using BilliardServer.Application.Abstractions.AsyncMessaging;
using BilliardServer.Application.Features.MatchShotsCalculate;
using BilliardServer.Application.Matches.Match;
using BilliardServer.Core.Common;
using BilliardServer.Core.Dto.Messaging.Responses.Match;
using Kborod.BilliardCore;
using Kborod.BilliardCore.Enums;
using Kborod.BilliardCore.Rules;
using Kborod.SharedDto;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BilliardServer.Application.Matches
{
    public class MatchControl : IDisposable
    {
        public readonly CreateMatchContext Context;
        private readonly IMediator _mediator;
        private readonly IMessagingResponseSenderService _responsesSender;
        private readonly ILogger _logger;

        private MatchBase _match;

        public MatchControl(IMediator mediator, IMessagingResponseSenderService responsesSender, ILogger logger, CreateMatchContext context)
        {
            Context = context;
            _mediator = mediator;
            _responsesSender = responsesSender;
            _logger = logger;
        }

        public async Task<Result> Init(List<UserProfileDto> profiles)
        {
            if (Context.GameType == GameType.PoolEight)
                _match = new MatchPoolEight(Context, _logger);
            else
                throw new NotImplementedException();

            foreach (var profile in profiles)
            {
                var startMatchData = new StartMatchData()
                {
                    MatchId = _match.Id,
                    GameType = _match.GameType,
                    BetType = Context.BetType,
                    Opponent = profiles.First(p => p.Id == _match.GetOpponent(profile.Id)),
                    BallsPosition = Context.PosNum,
                    TurningPlayerId = _match.TurningPlayer
                };

                await _responsesSender.SendResponseToUser(profile.Id, new MatchStartedResponseDto(startMatchData));
            }

            return Result.Ok();
        }

        public void PeriodicCheck(long timestamp)
        {
            if (timestamp < _match.StateEndTimestamp)
                return;

            if (_match.State == MatchState.WaitingPlayersInit)
            {
                //TODO BORODIN добавить реализацию
            }
            if (_match.State != MatchState.PrepeareTurn)
            {
                //TODO BORODIN добавить реализацию
            }
        }

        public Result MatchInitedReceived(string playerId)
        {
            if (_match.State != MatchState.WaitingPlayersInit)
                return Result.Fail($"Match state != {MatchState.WaitingPlayersInit}");

            _match.SetPlayerInited(playerId);
            TryCompleteTurn();

            return Result.Ok();
        }

        public Result AimInfoReceived(string playerId, AimInfoData data)
        {
            if (_match.State != MatchState.PrepeareTurn || _match.TurningPlayer != playerId)
                return Result.Fail("Ignored");

            _responsesSender.SendResponseToUser(_match.GetOpponent(playerId), new AimInfoResponseDto(data));

            return Result.Ok();
        }

        public Result MakeShotReceived(string playerId, MakeShotData data)
        {
            if (_match.State != MatchState.PrepeareTurn || _match.TurningPlayer != playerId)
                return Result.Fail("Ignored");

            if (ValidateCueBeforeShot(playerId, data.AimInfo.CueId) == false)
                return Result.Fail("Cue error");

            _ = _responsesSender.SendResponseToUser(_match.GetOpponent(playerId), new MakeShotResponseDto(data));

            var calculateContext = _match.GetShotContext(data.AimInfo, GetCuePower(data.AimInfo.CueId));

            _ = _mediator.Send(new CalculateShotCommand(calculateContext));

            return Result.Ok();
        }

        public Result ShotResultReceived(string playerId, SynchronizationInfo data)
        {
            _logger.LogWarning($"ShotResultReceived received {playerId}");

            if (_match.State != MatchState.WaitingTurnResults)
                return Result.Fail("Ignored");

            _match.SetPlayerTurnResult(playerId, data.RulesShotResult);
            TryCompleteTurn();

            return Result.Ok();
        }

        public void ShotCalculated(ITurnResult turnResult)
        {
            _logger.LogWarning("ShotCalculated received");
            if (turnResult == null)
            {
                ProcessMatchDesync(ShotValidateResult.FullDesync);
                return;
            }
            _match.SetCalculatedTurnResult(turnResult);
            TryCompleteTurn();
        }

        public Task UserDisconnected(string userId)
        {
            //TODO BORODIN добавить реализацию
            return Task.CompletedTask;
        }

        private void TryCompleteTurn()
        {
            _logger.LogWarning("TrySendNewTurnToPlayers");
            if (_match.State == MatchState.ShotValidationError)
            {
                _logger.LogCritical($"Validation result: {_match.LastShotValidationResult}");
                _logger.LogCritical($"Validation desync log: {_match.LastShotValidationDesyncLog}");
            }
            else if (_match.State == MatchState.PrepeareTurn)
            {
                var startTurnData = new StartTurnData()
                {
                    MatchId = _match.Id,
                    TurningPlayerId = _match.TurningPlayer,
                    TurnEndTimestamp = _match.StateEndTimestamp,
                };
                _ = _responsesSender.SendResponseToUser(_match.Player1, new StartTurnResponseDto(startTurnData));
                _ = _responsesSender.SendResponseToUser(_match.Player2, new StartTurnResponseDto(startTurnData));
            }
        }

        private float GetCuePower(int cueId)
        {
            //TODO BORODIN добавить реализацию
            return 300;
        }

        private bool ValidateCueBeforeShot(string playerId, int cueId)
        {
            //TODO BORODIN добавить реализацию
            return true;
        }

        private void ProcessMatchDesync(ShotValidateResult validateResult)
        {
            //TODO BORODIN добавить реализацию
            _logger.LogError($"[MatchControl] matchId  {_match.Id} desync: {validateResult}");
        }

        public void Dispose()
        {
            _logger.LogError($"[MatchControl] match {_match.Id} disposed");
        }
    }
}
