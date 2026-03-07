namespace Kborod.BilliardCore
{
    public class BallData : IBallData
	{
		public int Number { get; set; }
		public bool IsRemoved { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public void SetPosition(float x, float y)
        {
            X = x; Y = y;
        }

        public override string ToString()
        {
            return $"Num:{Number}_IsRemoved:{IsRemoved}_X:{X}_Y:{Y}";
        }
    }
}
