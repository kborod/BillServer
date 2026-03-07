using Kborod.BilliardCore.Enums;
using Kborod.BilliardCore.Rules;
using Kborod.BilliardCore.Rules.PoolEight;

namespace Kborod.BilliardCore
{
    public static class MatchExtensions
    {
        public static PoolBallType GetPoolBallType(this int ballNum)
        {
            if (ballNum <= 7)
                return PoolBallType.Solid;
            else if (ballNum >= 9)
                return PoolBallType.Striped;
            else
                return PoolBallType.None;
        }

        public static PoolBallType GetOpposite(this PoolBallType ballType)
        {
            if (ballType == PoolBallType.Solid)
                return PoolBallType.Striped;
            else if (ballType == PoolBallType.Striped)
                return PoolBallType.Solid;
            return PoolBallType.None;
        }

        public static PoolEightTurnResults CompleteTurnWithRules_P8(
            this ShotResult shotResult,
            Engine engine,
            string turningPlayerId,
            PoolBallType turnPlayerBallType,
            string oppPlayerId,
            bool isFirstSHot
            )
        {
            var poolEightRulesResult = PoolEightRules.ProcessShot(shotResult, isFirstSHot, engine.Balls, turnPlayerBallType);

            poolEightRulesResult.ReturnedBalls.ForEach(ball => { engine.ReturnPocketedBall(ball); });

            var nextTurningPlayer = !poolEightRulesResult.GameOver && poolEightRulesResult.TurnTransition ? oppPlayerId : turningPlayerId;
            var winUserId = poolEightRulesResult.GameOver
                    ? poolEightRulesResult.UserWin ? turningPlayerId : oppPlayerId
                    : null;

            var rulesResult = new RulesShotResult(
                poolEightRulesResult.Foul,
                engine.GetBallDatas(),
                shotResult.PocketedBalls,
                poolEightRulesResult.ReturnedBalls,
                turningPlayerId,
                nextTurningPlayer,
                winUserId
                );

            var nextTurningPlayerBallType = PoolBallType.None;
            if (poolEightRulesResult.BallTypeSelected != PoolBallType.None)
            {
                nextTurningPlayerBallType = poolEightRulesResult.BallTypeSelected;
            }
            else if (turnPlayerBallType != PoolBallType.None)
            {
                nextTurningPlayerBallType = nextTurningPlayer == turningPlayerId ? turnPlayerBallType : turnPlayerBallType.GetOpposite();
            }

            var nextTurnSettings = PoolEightRules.GetTurnSettings(engine.Balls, nextTurningPlayerBallType, poolEightRulesResult.Foul != FoulType.None);

            return new PoolEightTurnResults(poolEightRulesResult, rulesResult, nextTurnSettings);
        }
    }
}
