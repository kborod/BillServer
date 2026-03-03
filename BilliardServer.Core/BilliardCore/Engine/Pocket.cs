namespace Kborod.BilliardCore
{
    public class Pocket
    {
        public int pocketNum;
		public float x;
		public float y;

        //шар забит:
        /// <summary>
        /// Радиус лузы
        /// </summary>
        public float r;
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
		public Pocket(int pocketNum, float x, float y, float r, float vx, float vy)
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
