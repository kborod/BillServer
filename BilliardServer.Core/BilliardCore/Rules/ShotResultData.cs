using Kborod.BilliardCore;
using Kborod.BilliardCore.Enums;
using System.Collections.Generic;

namespace Kborod.BilliardCore.Rules
{
    public class ShotResultData
    {
        public ShotResult ShotResult { get; private set; }

        public FoulType Foul { get; private set; } = FoulType.None;

        public List<int> ReturnedPocketedBalls { get; private set; }

        public bool UserWin { get; private set; }

        public ShotResultData(ShotResult shotResult, FoulType foul, List<int> returnedPocketedBalls, bool userWin)
        {
            ShotResult = shotResult;
            Foul = foul;
            ReturnedPocketedBalls = returnedPocketedBalls;
            UserWin = userWin;
        }
    }
}
