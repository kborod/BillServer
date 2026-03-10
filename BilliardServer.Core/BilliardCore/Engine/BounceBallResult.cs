namespace Kborod.BilliardCore
{
    public struct BounceBallResult
    {
        public Fixed64 vx;
		public Fixed64 vy;

        /// <summary>
        /// Сила столкновения от 0 до 1
        /// </summary>
        public Fixed64 power;
    }
}
