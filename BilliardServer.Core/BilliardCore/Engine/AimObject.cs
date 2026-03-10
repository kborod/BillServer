namespace Kborod.BilliardCore
{
    public class AimObject
    {
		/// <summary>
		/// Координата X прицельного шара (начальная точка для отрисовки линии удара)s
		/// </summary>
		public Fixed64 AimBallX0;
		/**
		 * Координата Y прицельного шара (начальная точка для отрисовки линии удара)
		 */
		public Fixed64 AimBallY0;
        /**
		 * Координата X положения шара в момент соприкосновения (конечная точка для отрисовки линии удара)
		 */
        public Fixed64 AimBallX;
		/**
		 * Координата Y положения шара в момент соприкосновения (конечная точка для отрисовки линии удара)
		 */
		public Fixed64 AimBallY;
		/**
		 * Номер шара, с которым сталкивается сначала (если стена, то -1)
		 */
		public int FirstCollBallNum = -1;
		/**
		 * Координата Х прицельного шара
		 */
		public Fixed64 CollBallX0;
		/**
		 * Координата Y прицельного шара
		 */
		public Fixed64 CollBallY0;
		/**
		 * Компонента X вектора отскока битка
		 */
		public Fixed64 AimBallBounceVx;
		/**
		 * Компонента Y вектора отскока битка
		 */
		public Fixed64 AimBallBounceVy;
		/**
		 * Компонента X вектора отскока прицельного шара
		 */
		public Fixed64 CollBallBounceVx;
		/**
		 * Компонента Y вектора отскока прицельного шара
		 */
		public Fixed64 CollBallBounceVy;
    }
}
