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

            if (aimInfo.CueBallXraw.HasValue && aimInfo.CueBallYraw.HasValue)
            {
                if (_engine.ReplaceBall(aimInfo.CueBall.Value, new Fixed64(aimInfo.CueBallXraw.Value), new Fixed64(aimInfo.CueBallYraw.Value),
                    context.OnlyKitchen, correctionAllowed: true) == false)
                {
                    throw new Exception("ReplaceBall error");
                }
            }
            _engine.MakeShot(
                new Fixed64(aimInfo.DirectionXraw) * new Fixed64(aimInfo.PowerRaw) * Fixed64.FromInt(context.CuePower),
                new Fixed64(aimInfo.DirectionYraw) * new Fixed64(aimInfo.PowerRaw) * Fixed64.FromInt(context.CuePower),
                aimInfo.CueBall.Value,
                new Fixed64(aimInfo.SpinXraw),
                new Fixed64(aimInfo.SpinYraw));

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
