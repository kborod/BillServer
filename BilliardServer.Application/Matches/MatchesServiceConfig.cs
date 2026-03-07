namespace BilliardServer.Application.Matches
{
    public class MatchesServiceConfig
    {
        public const string SectionName = "MatchesServiceConfig";

        public float CheckMatchesPeriodSeconds { get; set; } = 3f;
    }
}
