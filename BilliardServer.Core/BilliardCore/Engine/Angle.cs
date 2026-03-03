namespace Kborod.BilliardCore
{
    public class Angle
    {
        public float x { get; private set; }
		public float y { get; private set; }
        public float r { get; private set; }

        public Angle(float x, float y, float r)
        {
            this.x = x;
            this.y = y;
            this.r = r;
        }
    }
}
