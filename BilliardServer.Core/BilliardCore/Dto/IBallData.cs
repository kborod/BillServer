namespace Kborod.BilliardCore
{
    public interface IBallData
    {
		public int Number { get; }
		public bool IsRemoved { get; }
        public float X { get; }
        public float Y { get; }
    }
}
