namespace Kborod.BilliardCore
{
    public class Pocket
    {
        public int pocketNum;
		public Fixed64 x;
		public Fixed64 y;

        //шар забит:
        /// <summary>
        /// Радиус лузы
        /// </summary>
        public Fixed64 r;
        /// <summary>
        /// Вектор "скатывания" шара под стол
        /// </summary>
        public MyVector vRemove;
        /// <summary>
        /// Точка, после отскока от которой шар скатывается под стол.
        /// </summary>
        public Point pRemove;
		/**
		 * Луза
		 * @param	pocketNum
		 * @param	x
		 * @param	y
		 * @param	r
		 * @param	vx Вектор для анимирования "Шар в лузе", в направлении которого шар уходит из лузы
		 * @param	vy Вектор для анимирования "Шар в лузе", в направлении которого шар уходит из лузы
		 */
		public Pocket(int pocketNum, Fixed64 x, Fixed64 y, Fixed64 r, Fixed64 vx, Fixed64 vy)
        {
            this.pocketNum = pocketNum;
            this.x = x;
            this.y = y;
            this.r = r;
            
            vRemove = new MyVector();
            vRemove.vx = vx;
            vRemove.vy = vy;
            vRemove.updatePointsFromComponents();
            vRemove.makeVector();
            pRemove = new Point(x + (-vRemove.dx * (r - Config.BALL_RAD_PX)), y + (-vRemove.dy * (r - Config.BALL_RAD_PX)));
        }
    }
}
