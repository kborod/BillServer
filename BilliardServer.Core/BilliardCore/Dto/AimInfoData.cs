namespace Kborod.BilliardCore
{
    public struct AimInfoData
    {
        public string MatchId { get; set; }
        public AimInfo AimInfo { get; set; }

        public AimInfoData(string matchId, AimInfo aimInfo)
        {
            MatchId = matchId;
            AimInfo = aimInfo;
        }
    }
}