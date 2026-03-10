namespace Kborod.BilliardCore
{
    public class Angle
    {
        public Fixed64 x { get; private set; }
		public Fixed64 y { get; private set; }
        public Fixed64 r { get; private set; }

        public Angle(Fixed64 x, Fixed64 y, Fixed64 r)
        {
            this.x = x;
            this.y = y;
            this.r = r;
        }
    }
}
