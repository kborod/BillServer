namespace Kborod.BilliardCore
{
    public struct MakeShotData
    {
        public string MatchId { get; set; }
        public AimInfo AimInfo { get; set; }

        public MakeShotData(string matchId, AimInfo aimInfo)
        {
            MatchId = matchId;
            AimInfo = aimInfo;
        }
    }
}