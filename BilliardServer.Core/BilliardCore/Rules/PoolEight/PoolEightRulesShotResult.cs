using Kborod.BilliardCore.Enums;
using System.Collections.Generic;

namespace Kborod.BilliardCore.Rules.PoolEight
{
    public class PoolEightRulesShotResult
    {
        public List<int> ReturnedBalls;
        public bool TurnTransition;
        public FoulType Foul;
        public PoolBallType BallTypeSelected;
        public bool GameOver;
        public bool UserWin;
    }
}
