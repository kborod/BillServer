namespace Kborod.BilliardCore
{
    /// <summary> Объект с информацией о пересечении луча и отрезка (вектора скорости шара и стенки стола) </summary>
    public class BallWallIntersectResult
    {

        /// <summary> Признак, пересекается ли траектория шара и стена </summary>
        public bool IsIntersect => u1 >= Fixed64.Zero && u2 >= Fixed64.Zero && u2 <= Fixed64.One;

        /// <summary> если P2 шара слева от вектора стены (векторное произведение < 0), значит надо обрабатывать коллизию, иначе коллизия уже обработана и шар уже движется в направлении от стены  </summary>
        public bool IsNeedProcessCollision => vectorProductZcoord > Fixed64.Zero;

        /// <summary>
        /// Координата Х точки пересечения
        /// </summary>
        public Fixed64 x = Fixed64.FromFloat(0);

        /// <summary>
        /// Координата Y точки пересечения
        /// </summary>
        public Fixed64 y = Fixed64.FromFloat(0);

        /// <summary>
        /// Коэффициент единичного вектора шара до точки пересечения
        /// </summary>
        public Fixed64 u1 = Fixed64.MaxValue;

        /// <summary>
        /// Коэффициент единичного вектора стены до точки пересечения
        /// </summary>
        public Fixed64 u2 = Fixed64.MaxValue;


        /// <summary>
        /// векторное произведение
        /// </summary>
        public Fixed64 vectorProductZcoord = Fixed64.Zero;

        public void clear()
		{
			/*isLinesIntersect = false;
			isStretchIntersect = false;
			isRayIntersect = false;*/
			x = Fixed64.Zero;
			y = Fixed64.Zero;
			u1 = Fixed64.MaxValue;
            u2 = Fixed64.MaxValue;
            vectorProductZcoord = Fixed64.Zero;
		}

    }
}
