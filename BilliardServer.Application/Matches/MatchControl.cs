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

        private PlayersInitResults _playersInitResults = new PlayersInitResults();
        private MakeShotResults _turnResults = new MakeShotResults();

        private object _lockObj = new object();

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

            lock(_lockObj)
            {
                if (playerId == _match.Player1)
                    _playersInitResults.SetPlayer1Inited();
                else
                    _playersInitResults.SetPlayer2Inited(); 
                
                if (_playersInitResults.IsAllClientsInited() == true)
                {
                    _match.PlayersInitedHandler();
                    StartTurn();
                }
            }

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

            var calculateContext = _match.GetContextForCalculateShot(data.AimInfo, GetCuePower(data.AimInfo.CueId));

            _ = _mediator.Send(new CalculateShotCommand(calculateContext));

            return Result.Ok();
        }

        public Result ShotResultReceived(string playerId, SynchronizationInfo data)
        {
            if (_match.State != MatchState.WaitingTurnResults)
            {
                MatchDesynced(playerId == _match.Player1 ? ShotValidateResult.Player1Desync : ShotValidateResult.Player2Desync);
                return Result.Fail($"error: shotResult recieved in match state  ({_match.State})");
            }

            lock (_lockObj)
            {
                if (playerId == _match.Player1)
                    _turnResults.SetPlayer1Result(data.RulesShotResult);
                else
                    _turnResults.SetPlayer2Result(data.RulesShotResult);

                TryCompleteTurn();
            }

            return Result.Ok();
        }

        public void ShotCalculated(ITurnResult turnResult)
        {
            if (turnResult == null)
            {
                _logger.LogCritical("[MatchControl] ShotCalculated received is null");
                MatchDesynced(ShotValidateResult.FullDesync);
                return;
            }

            if (_match.State != MatchState.WaitingTurnResults)
            {
                _logger.LogCritical($"[MatchControl] TurnResult received when state is {_match.State}");
                MatchDesynced(ShotValidateResult.FullDesync);
                return;
            }

            _turnResults.SetCalculateResult(turnResult);
            TryCompleteTurn();
        }

        public Task UserDisconnected(string userId)
        {
            //TODO BORODIN добавить реализацию
            return Task.CompletedTask;
        }

        private void TryCompleteTurn()
        {
            if (_turnResults.IsAllResultsReceived() == false)
                return;

            if (_turnResults.Validate() != ShotValidateResult.Ok)
            {
                _logger.LogCritical($"Validation result: {_turnResults.LastValidateResult}\nValidation desync log: {_turnResults.DesyncLog}");
                MatchDesynced(_turnResults.LastValidateResult);
                return;
            }

            _match.ProcessTurnResult(_turnResults.CalculatedResult!);

            if (_match.State == MatchState.PrepeareTurn)
            {
                StartTurn();
            }
            else if (_match.State == MatchState.Over)
            {
                MatchOver(_turnResults.CalculatedResult!);
            }

            _turnResults.Clear();
        }

        private int GetCuePower(int cueId)
        {
            //TODO BORODIN добавить реализацию
            return 300;
        }

        private bool ValidateCueBeforeShot(string playerId, int cueId)
        {
            //TODO BORODIN добавить реализацию
            return true;
        }

        private void StartTurn()
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

        private void MatchOver(ITurnResult turnResult)
        {
            //TODO BORODIN добавить реализацию
            _logger.LogError($"[MatchControl] matchId  {_match.Id} over. WinnerId: {turnResult.RulesResult.WinUserIdOrNull}");
        }

        private void MatchDesynced(ShotValidateResult validateResult)
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
