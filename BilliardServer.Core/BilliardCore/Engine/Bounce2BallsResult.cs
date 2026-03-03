namespace Kborod.BilliardCore
{
    public struct Bounce2BallsResult
    {
        public float vx1;
		public float vy1;
		public float vx2;
		public float vy2;

        /// <summary>
        /// Сила столкновения от 0 до 1
        /// </summary>
        public float power;
    }
}
