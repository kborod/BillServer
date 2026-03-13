using System.Collections.Generic;

namespace Kborod.BilliardCore
{
    public interface IEngineForUI
    {
		public List<Ball> Balls { get; }
		public List<MyVector> RealWalls { get; }

		public (bool result, Fixed64 x, Fixed64 y) GetReplacedPosition(int ballNum, Fixed64 posX, Fixed64 posY, bool onlyKitchen, bool correctionAllowed);
		public bool ReplaceBall(int ballNum, Fixed64 posX, Fixed64 posY, bool onlyKitchen, bool correctionAllowed);

		public void ReturnPocketedBall(int ballNum);

		public AimObject GetAimObject(Ball b);
    }
}
