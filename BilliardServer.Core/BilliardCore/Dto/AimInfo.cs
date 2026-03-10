namespace Kborod.BilliardCore
{
    public struct AimInfo
    {
        public int? CueBall { get; set; }
        public long DirectionXraw { get; set; }
        public long DirectionYraw { get; set; }
        public int? Pocket { get; set; }

        public long? CueBallXraw { get; set; }
        public long? CueBallYraw { get; set; }

        public long SpinXraw { get; set; }
        public long SpinYraw { get; set; }

        public long PowerRaw { get; set; }

        public int CueId { get; set; }

        public bool IsBallMovingNow { get; set; }
    }
}