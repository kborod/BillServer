namespace Kborod.BilliardCore
{
    public struct StartTurnData
    {
        public string MatchId { get; set; }
        public string TurningPlayerId { get; set; }
        public long TurnEndTimestamp { get; set; }
    }
}