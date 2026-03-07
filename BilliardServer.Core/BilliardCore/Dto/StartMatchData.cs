using Kborod.BilliardCore.Enums;
using Kborod.SharedDto;

namespace Kborod.BilliardCore
{
    public struct StartMatchData
    {
        public string MatchId { get; set; }
        public GameType GameType { get; set; }
        public BetType BetType { get; set; }
        public UserProfileDto Opponent { get; set; }
        public int BallsPosition { get; set; }
        public string TurningPlayerId { get; set; }
    }
}