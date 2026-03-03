using System.Collections.Generic;

namespace Kborod.BilliardCore
{
    public class Collisions
    {
		public float TimeToCollision;

        public List<Ball[]> BallsCollisions = new List<Ball[]>();
		public List<KeyValuePair<Ball, MyVector>> BallWallCollisions = new List<KeyValuePair<Ball, MyVector>>();
		public List<KeyValuePair<Ball, Angle>> BallAngleCollisions = new List<KeyValuePair<Ball, Angle>>();
		public List<KeyValuePair<Ball, Pocket>> BallPocketCollisions = new List<KeyValuePair<Ball, Pocket>>();

        public void Add2BallsCollision(Ball b1, Ball b2, float dt)
        {
            TryUpdateCollisionTime(dt);
            BallsCollisions.Add(new Ball[] { b1, b2 });
        }
		
		public void AddBallWithWallCollision(Ball b, MyVector w, float dt)
        {
            TryUpdateCollisionTime(dt);
            BallWallCollisions.Add(new KeyValuePair<Ball, MyVector>(b, w));
		}
		
		public void AddBallWithAngleCollision(Ball b, Angle a, float dt)
        {
            TryUpdateCollisionTime(dt);
            BallAngleCollisions.Add(new KeyValuePair<Ball, Angle>(b, a));
        }
		
		public void AddBallWithPocketCollision(Ball b, Pocket p, float dt)
		{
			TryUpdateCollisionTime(dt);
            BallPocketCollisions.Add(new KeyValuePair<Ball, Pocket>(b, p));
		}

		public void Clear()
		{
            TimeToCollision = float.MaxValue;

			BallsCollisions.Clear();
			BallWallCollisions.Clear();
			BallAngleCollisions.Clear();
			BallPocketCollisions.Clear();
		}

		private void TryUpdateCollisionTime(float newTime)
		{
			if (newTime < TimeToCollision)
			{
				Clear();
                TimeToCollision = newTime;
            }
		}
	}
}
