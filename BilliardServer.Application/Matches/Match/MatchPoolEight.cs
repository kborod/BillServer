using Kborod.BilliardCore;
using Kborod.BilliardCore.Enums;
using Kborod.BilliardCore.Rules;
using Kborod.BilliardCore.Rules.PoolEight;
using Microsoft.Extensions.Logging;

namespace BilliardServer.Application.Matches.Match
{
    public class MatchPoolEight : MatchBase
    {
        private PoolBallType _player1BallType = PoolBallType.None;

        public MatchPoolEight(CreateMatchContext context, ILogger logger) : base(context, logger) { }

        protected override void Init(out List<BallData> ballDatas, out TurnSettings firstTurnSettings, int posNum)
        {
            ballDatas = Config.GetBallsPositionsForNewGame(GameType.PoolEight, posNum);
            firstTurnSettings = PoolEightRules.GetFirstTurnSettings(BallDatas);
        }

        public override ICalculateContext GetContextForCalculateShot(AimInfo aimInfo, int cuePower)
        {
            ChangeState(MatchState.WaitingTurnResults);
            return new CalculatePoolShotContext(Id, GameType, BallDatas, aimInfo, TurningPlayer,
                GetPlayerBallType(TurningPlayer), GetOpponent(TurningPlayer), MatchShotsCount == 0, TurnSettings.MoveOnlyInKitchen, cuePower);
        }

        public override void ProcessTurnResult(ITurnResult turnResult)
        {
            if ((turnResult is PoolEightTurnResults p8result) == false)
                throw new Exception($"MatchPoolEight.ShotCompletedHandler turnResult type is {turnResult.GetType()}");

            TrySelectBallTypes(
                p8result.RulesResult.CurrTurnPlayerId,
                p8result.PoolEightRulesResult.BallTypeSelected);

            base.ProcessTurnResult(turnResult);
        }

        private void TrySelectBallTypes(string playerId, PoolBallType selectedBallType)
        {
            if (_player1BallType != PoolBallType.None || selectedBallType == PoolBallType.None)
                return;

            _player1BallType = playerId == Player1 ? selectedBallType : selectedBallType.GetOpposite();
        }

        private PoolBallType GetPlayerBallType(string playerId)
        {
            if (_player1BallType == PoolBallType.None)
                return PoolBallType.None;

            return playerId == Player1 ? _player1BallType : _player1BallType.GetOpposite();
        }
    }
}
