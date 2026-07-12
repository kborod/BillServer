namespace BilliardServer.Core.Dto.Leaderboard
{
    public class LeaderboardEntryDto
    {
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Avatar { get; set; }
        public int Position { get; set; }
    }
}
