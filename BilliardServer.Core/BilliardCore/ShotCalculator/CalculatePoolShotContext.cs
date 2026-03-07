using Kborod.BilliardCore.Enums;
using System.Collections.Generic;

namespace Kborod.BilliardCore
{
    public record CalculatePoolShotContext : ICalculateContext
    {
        public string MatchId { get; }
        public GameType GameType { get; }
        public List<BallData> BallDatas { get; }
        public AimInfo AimInfo { get; }
        public string TurningPlayerId { get; }
        public PoolBallType TurningPlayerBallType { get; }
        public string OppPlayerId { get; }
        public bool IsFirstSHot { get; }
        public bool OnlyKitchen { get; }
        public float CuePower { get; }

        public CalculatePoolShotContext(string matchId, GameType gameType, List<BallData> ballDatas, 
            AimInfo aimInfo, string turningPlayerId, PoolBallType turningPlayerBallType, string oppPlayerId, 
            bool isFirstSHot, bool onlyKitchen, float cuePower)
        {
            MatchId = matchId;
            GameType = gameType;
            BallDatas = ballDatas;
            AimInfo = aimInfo;
            TurningPlayerId = turningPlayerId;
            TurningPlayerBallType = turningPlayerBallType;
            OppPlayerId = oppPlayerId;
            IsFirstSHot = isFirstSHot;
            OnlyKitchen = onlyKitchen;
            CuePower = cuePower;
        }
    }
}
