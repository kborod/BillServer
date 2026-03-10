using System.Collections.Generic;

namespace Kborod.BilliardCore
{
    public interface IEngineForUI
    {
		public List<Ball> Balls { get; }
		public List<MyVector> RealWalls { get; }

		//public void SetBallPosition(int ballNum, float posX, float posY);

		public bool ReplaceBall(int ballNum, Fixed64 posX, Fixed64 posY, bool onlyKitchen, bool correctionAllowed);

		public void ReturnPocketedBall(int ballNum);

		public AimObject GetAimObject(Ball b);
    }
}
