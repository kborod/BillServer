namespace Kborod.BilliardCore
{
    public struct AimInfo
    {
        public int? CueBall { get; set; }
        public float DirectionX { get; set; }
        public float DirectionY { get; set; }
        public int? Pocket { get; set; }

        public float? CueBallX { get; set; }
        public float? CueBallY { get; set; }

        public float SpinX { get; set; }
        public float SpinY { get; set; }

        public float Power { get; set; }

        public int CueId { get; set; }

        public bool IsBallMovingNow { get; set; }
    }
}