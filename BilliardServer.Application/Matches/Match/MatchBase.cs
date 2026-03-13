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

        public long StateEndTimestamp { get; protected set; }

        protected int MatchShotsCount = 0;

        protected const int InitClientsStateSeconds = 20;
        protected const int PrepeareTurnStateSeconds = 2000;
        protected const int WaitTurnResultsStateSeconds = 20;

        protected readonly ILogger _logger;

        protected MatchBase(MatchContext context, ILogger logger)
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

            StateEndTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + InitClientsStateSeconds;

            ChangeState(MatchState.WaitingPlayersInit);
        }

        public abstract ICalculateContext GetContextForCalculateShot(AimInfo aimInfo, int cuePower);

        public abstract int GetScore(string playerId);

        public void PlayersInitedHandler()
        {
            StateEndTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + PrepeareTurnStateSeconds;
            ChangeState(MatchState.PrepeareTurn);
        }

        public virtual void ProcessTurnResult(ITurnResult result)
        {
            BallDatas = result.RulesResult.BallDatas;

            if (result.RulesResult.WinUserIdOrNull != null)
            {
                ChangeState(MatchState.Over);
            }
            else
            {
                TurningPlayer = result.RulesResult.NextTurnPlayerId;
                TurnSettings = result.NextTurnSettings;
                StateEndTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + PrepeareTurnStateSeconds;

                MatchShotsCount++;

                ChangeState(MatchState.PrepeareTurn);
            }
        }

        public string GetOpponent(string playerId)
        {
            return playerId == Player1 ? Player2 : Player1;
        }

        protected void ChangeState(MatchState state)
        {
            State = state;
        }

        protected abstract void Init(out List<BallData> ballDatas, out TurnSettings firstTurnSettings, int posNum);
    }
}
