namespace Kborod.BilliardCore
{
    /// <summary> Объект с информацией о пересечении луча и отрезка (вектора скорости шара и стенки стола) </summary>
    public class BallWallIntersectResult
    {

        /// <summary> Признак, пересекается ли траектория шара и стена </summary>
        public bool IsIntersect => u1 >= 0 && u2 >= 0 && u2 <= 1;

        /// <summary> если P2 шара слева от вектора стены (векторное произведение < 0), значит надо обрабатывать коллизию, иначе коллизия уже обработана и шар уже движется в направлении от стены  </summary>
        public bool IsNeedProcessCollision => vectorProductZcoord > 0;

        /// <summary>
        /// Координата Х точки пересечения
        /// </summary>
        public float x = 0;

        /// <summary>
        /// Координата Y точки пересечения
        /// </summary>
        public float y = 0;

        /// <summary>
        /// Коэффициент единичного вектора шара до точки пересечения
        /// </summary>
        public float u1 = float.MaxValue;

        /// <summary>
        /// Коэффициент единичного вектора стены до точки пересечения
        /// </summary>
        public float u2 = float.MaxValue;


        /// <summary>
        /// векторное произведение
        /// </summary>
        public float vectorProductZcoord = 0;

        public void clear()
		{
			/*isLinesIntersect = false;
			isStretchIntersect = false;
			isRayIntersect = false;*/
			x = 0;
			y = 0;
			u1 = float.MaxValue;
            u2 = float.MaxValue;
            vectorProductZcoord = 0;
		}

        public string UtoString => u1.ToString("N8");

    }
}
