using System;
using System.Collections.Generic;

namespace Kborod.BilliardCore
{
    public class ShotCalculator
    {
        private Engine _engine = new Engine();

        public ShotResult CalculateShot(
            List<BallData> balls, int cueBallNum, float vx, float vy,
                int newPosBallNum, float bx, float by, bool onlyKitchen,
                float spinVx, float spinVy)
        {
            _engine.SetBallsPosition(balls);
            if (newPosBallNum >= 0 && bx != -1 && by != -1)
            {
                if (_engine.ReplaceBall(newPosBallNum, bx, by, onlyKitchen, correctionAllowed: false) == false)
                    throw new Exception("ReplaceBall error");
            }
            _engine.MakeShot(vx, vy, cueBallNum, spinVx, spinVy);
            _engine.UpdateModel(int.MaxValue, out var tickResult, out var shotResult);
            return shotResult;
        }

    }
}
