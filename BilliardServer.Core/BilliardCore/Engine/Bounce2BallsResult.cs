namespace Kborod.BilliardCore
{
    public struct Bounce2BallsResult
    {
        public Fixed64 vx1;
		public Fixed64 vy1;
		public Fixed64 vx2;
		public Fixed64 vy2;

        /// <summary>
        /// Сила столкновения от 0 до 1
        /// </summary>
        public Fixed64 power;
    }
}
