namespace BilliardServer.Application.Leaderboard
{
    public class LeaderboardServiceConfig
    {
        public const string SectionName = "LeaderboardServiceConfig";

        public string LeaderboardRatingKey { get; set; } = "leaderboardRate:top";
        public string LeaderboardExpKey { get; set; } = "leaderboardExp:top";
        public bool NeedRefreshAtStart { get; set; } = false;
    }
}
