using System.Collections.Generic;

namespace Kborod.BilliardCore
{
    public struct ShotTickResult
    {
		public int DeltaTimeMS;

		/**
		 * Маскимальная сила столкновения с бортами (0:1) либо 0, если не было столкновений
		 */
		public Fixed64 MaxWallsCollPower { get; private set; }

        /**
		 * Массив, значениями которого являются силы столкновений шара с шаром. Если за тик
		 * произошло два столкновения шаров с силами 0,5 и 0,3 то массив будет [0.5 и 0.3]
		 */
        public Fixed64 MaxBallsCollPower { get; private set; }

        /**
		 * Массив, значениями которого являются силы забитых шаров. Если за тик
		 * забито два шара с силами 0,5 и 0,3 то массив будет [0.5 и 0.3]
		 */
        public Fixed64 MaxPocketedPower { get; private set; }

		public List<Ball> PocketedBallsOrNull { get; private set; }

        public void SetDeltaTime(int deltaTimeMS)
        {
            DeltaTimeMS = deltaTimeMS;
        }

        public void TryChangeMaxWallsCollPower(Fixed64 power)
		{
			if (MaxWallsCollPower < power)
				MaxWallsCollPower = power;
		}

		public void TryChangeMaxBallsCollPower(Fixed64 power)
		{
			if (MaxBallsCollPower < power)
                MaxBallsCollPower = power;
		}

		public void TryChangeMaxPocketedPower(Fixed64 power)
		{
			if (MaxPocketedPower < power)
                MaxPocketedPower = power;
		}

		public void AddPocketedBall(Ball ball)
		{
			PocketedBallsOrNull ??= new List<Ball>();
			PocketedBallsOrNull.Add(ball);
		}
    }
}
