namespace Kborod.BilliardCore
{
    /// <summary> Информация для синхронизации состояния после удара </summary>
    public struct SyncronizationInfo
    {
        public string MatchId { get; set; }
        public List<BallData> Balls { get; set; }
        public string TurningPlayerId { get; set; }
        public string WinPlayerId { get; set; }
    }
}