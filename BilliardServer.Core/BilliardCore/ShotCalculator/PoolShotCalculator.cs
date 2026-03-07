using System;

namespace Kborod.BilliardCore
{
    public class PoolShotCalculator
    {
        private Engine _engine = new Engine();

        public PoolEightTurnResults CalculateShot(CalculatePoolShotContext context)
        {
            var aimInfo = context.AimInfo;

            if (aimInfo.CueBall.HasValue == false)
            {
                throw new Exception("cueBallNum is null");
            }

            _engine.SetBallDatas(context.BallDatas);

            if (aimInfo.CueBallX.HasValue && aimInfo.CueBallY.HasValue)
            {
                if (_engine.ReplaceBall(aimInfo.CueBall.Value, aimInfo.CueBallX.Value, aimInfo.CueBallY.Value,
                    context.OnlyKitchen, correctionAllowed: true) == false)
                {
                    throw new Exception("ReplaceBall error");
                }
            }
            _engine.MakeShot(
                aimInfo.DirectionX * aimInfo.Power * context.CuePower,
                aimInfo.DirectionY * aimInfo.Power * context.CuePower,
                aimInfo.CueBall.Value,
                aimInfo.SpinX,
                aimInfo.SpinY);

            _engine.UpdateModel(int.MaxValue, out var tickResult, out var shotResult);

            var result = shotResult.CompleteTurnWithRules_P8(
                _engine,
                context.TurningPlayerId,
                context.TurningPlayerBallType,
                context.OppPlayerId,
                context.IsFirstSHot
                );

            return result;
        }
    }
}
