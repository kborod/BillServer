namespace Kborod.BilliardCore
{
    public struct MatchOverData
    {
        public string MatchId { get; set; }
        public string WinPlayerIdOrNull { get; set; }
        public int WinPlayerScore { get; set; }
        public int LosePlayerScore { get; set; }
        public bool AfterServerError { get; set; }
    }
}