namespace Kborod.BilliardCore
{
    public interface IBallData
    {
		public int Number { get; }
		public bool IsRemoved { get; }
        public long Xraw { get; }
        public long Yraw { get; }
    }
}
