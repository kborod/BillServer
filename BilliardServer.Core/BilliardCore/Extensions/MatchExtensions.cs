using Kborod.BilliardCore.Enums;

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
    }
}
