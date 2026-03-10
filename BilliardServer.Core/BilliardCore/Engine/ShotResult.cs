using System.Collections.Generic;

namespace Kborod.BilliardCore
{
    public class ShotResult
    {
        /// <summary>
        /// Номер шара, с которым произошло первое касание
        /// </summary>
        public int? FirstCollisionBallNum = null;

        /// <summary>
        /// Список забитых шаров в порядке забивания
        /// </summary>
        public List<int> PocketedBalls = new List<int>();

        /// <summary>
        /// Список луз, в которые забиты шары, параллельно pocketedBalls
        /// </summary>
        public List<int> PocketedBallsPockets = new List<int>();

        /// <summary>
        /// Количество столкновений между шарами
        /// </summary>
        public int BallsCollisionCount = 0;

        /// <summary>
        /// Всего количество столкновений с бортами
        /// </summary>
        public int WallsCollisionCount = 0;
        
        /// <summary>
        /// Количество столкновений с бортами после первого соударения с шаром
        /// </summary>
        public int WallsCollisionAfterBallsCollisionCount = 0;

        /// <summary>
        /// Продолжительность удара
        /// </summary>
        public double ShotDuration;
		
		public override string ToString()
		{
			return "ShotCalcResult:duration:" + ShotDuration + ";firstCollisionBallNum:" + FirstCollisionBallNum + ";wallsCollisionCount:" + WallsCollisionCount 
				+ ";pocketedBalls:" + PocketedBalls + ";pocketedBallsPockets:" + PocketedBallsPockets;
		}
}
}
