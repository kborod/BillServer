using Kborod.BilliardCore.Enums;

namespace Kborod.BilliardCore
{
    public struct MatchOverData
    {
        public string MatchId { get; set; }
        public GameType GameType { get; set; }
        public BetType BetType { get; set; }
        public string WinPlayerIdOrNull { get; set; }
        public int WinnerScore { get; set; }
        public int LoserScore { get; set; }
        public bool AfterServerError { get; set; }
        public bool RematchAvailable { get; set; }
    }
}