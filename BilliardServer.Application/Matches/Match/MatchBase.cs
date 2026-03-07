using Kborod.BilliardCore;
using Kborod.BilliardCore.Enums;
using Kborod.BilliardCore.Rules;
using Microsoft.Extensions.Logging;

namespace BilliardServer.Application.Matches.Match
{
    public abstract class MatchBase
    {
        public GameType GameType { get; }
        public string Id { get; }
        public string Player1 { get; }
        public string Player2 { get; }
        public string TurningPlayer { get; protected set; }

        public MatchState State { get; private set; } = MatchState.Initializing;
        public List<BallData> BallDatas { get; private set; }
        public TurnSettings TurnSettings { get; private set; }

        public long StateEndTimestamp { get; private set; }

        public ShotValidateResult LastShotValidationResult => _shotResults.LastValidateResult;
        public string LastShotValidationDesyncLog => _shotResults.DesyncLog;

        protected int MatchShotsCount = 0;

        private const int InitClientsWaitingSeconds = 20;
        private const int ShotDurationSeconds = 2000;

        protected readonly ILogger _logger;

        private PlayersInitResults _playersInitResults = new PlayersInitResults();
        private MakeShotResults _shotResults = new MakeShotResults();

        protected MatchBase(CreateMatchContext context, ILogger logger)
        {
            GameType = context.GameType;
            Id = context.Id;
            Player1 = context.Player1;
            Player2 = context.Player2;
            TurningPlayer = context.TurningPlayer;
            Init(out var startBallDatas, out var firstTurnSettings, context.PosNum);
            BallDatas = startBallDatas;
            TurnSettings = firstTurnSettings;

            _logger = logger;

            StateEndTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + InitClientsWaitingSeconds;

            ChangeState(MatchState.WaitingPlayersInit);
        }

        public void SetPlayerInited(string playerId)
        {
            if (playerId == Player1)
                _playersInitResults.SetPlayer1Inited();
            else
                _playersInitResults.SetPlayer2Inited();

            if (_playersInitResults.IsAllClientsInited() == false)
                return;

            StateEndTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ShotDurationSeconds;
            ChangeState(MatchState.PrepeareTurn);
        }

        public abstract ICalculateContext GetShotContext(AimInfo aimInfo, float cuePower);

        public void SetCalculatedTurnResult(ITurnResult turnResult)
        {
            _shotResults.SetCalculateResult(turnResult);

            TryCompleteTurn();
        }

        public void SetPlayerTurnResult(string playerId, RulesShotResult result)
        {
            if (playerId == Player1)
                _shotResults.SetPlayer1Result(result);
            else
                _shotResults.SetPlayer2Result(result);

            TryCompleteTurn();
        }

        public string GetOpponent(string playerId)
        {
            return playerId == Player1 ? Player2 : Player1;
        }

        private void TryCompleteTurn()
        {
            if (_shotResults.IsAllResultsReceived() == false)
            {
                return;
            }
            if (_shotResults.Validate() != ShotValidateResult.Ok)
            {
                ChangeState(MatchState.ShotValidationError);
            }
            else
            {
                ProcessTurnResult(_shotResults.CalculatedResult!);
            }
        }

        protected virtual void ProcessTurnResult(ITurnResult result)
        {
            TurningPlayer = result.RulesResult.NextTurnPlayerId;
            TurnSettings = result.NextTurnSettings;
            BallDatas = result.RulesResult.BallDatas;
            StateEndTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ShotDurationSeconds;

            _shotResults.Clear();

            if (result.RulesResult.WinUserIdOrNull != null)
                ChangeState(MatchState.Over);
            else
                ChangeState(MatchState.PrepeareTurn);
        }

        protected void ChangeState(MatchState state)
        {
            State = state;
        }

        protected abstract void Init(out List<BallData> ballDatas, out TurnSettings firstTurnSettings, int posNum);
    }
}
