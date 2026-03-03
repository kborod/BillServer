namespace Kborod.BilliardCore
{
    public class AimObject
    {
		/// <summary>
		/// Координата X прицельного шара (начальная точка для отрисовки линии удара)s
		/// </summary>
		public float AimBallX0;
		/**
		 * Координата Y прицельного шара (начальная точка для отрисовки линии удара)
		 */
		public float AimBallY0;
        /**
		 * Координата X положения шара в момент соприкосновения (конечная точка для отрисовки линии удара)
		 */
        public float AimBallX;
		/**
		 * Координата Y положения шара в момент соприкосновения (конечная точка для отрисовки линии удара)
		 */
		public float AimBallY;
		/**
		 * Номер шара, с которым сталкивается сначала (если стена, то -1)
		 */
		public int FirstCollBallNum = -1;
		/**
		 * Координата Х прицельного шара
		 */
		public float CollBallX0;
		/**
		 * Координата Y прицельного шара
		 */
		public float CollBallY0;
		/**
		 * Компонента X вектора отскока битка
		 */
		public float AimBallBounceVx;
		/**
		 * Компонента Y вектора отскока битка
		 */
		public float AimBallBounceVy;
		/**
		 * Компонента X вектора отскока прицельного шара
		 */
		public float CollBallBounceVx;
		/**
		 * Компонента Y вектора отскока прицельного шара
		 */
		public float CollBallBounceVy;
    }
}
