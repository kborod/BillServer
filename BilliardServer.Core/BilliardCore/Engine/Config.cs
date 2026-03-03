namespace Kborod.BilliardCore
{
    public class Config
    {
        public const float MODEL_COORD_TO_WORLD_KOEF = 0.0162f;

        // !!!250 максимальная длина вектора скорости шара
        public const float MAX_SHOT_POWER           = 300f;

        public const int SPEED_UPDATE_DELTA = 100;	//через каждый SPEED_UPDATE_DELTA происходит корректировка векторов движения (трение, вращение и т.д.)
		public const int BALLS_INTEGRATE_DELTA = 10;	//через каждый BALLS_INTEGRATE_DELTA происходит интегрирование модели

        public const float BALL_DIAM_PX 			= 18;
		public const float BALL_DIAM_PX_SQUARED 	= 18 * 18;
        public const float BALL_RAD_PX              = 9;
		public const float BALL_RAD_PX_SQUARED		= 9 * 9;

        public const float BALL_FRICTION 		    = 0.04f;    //!!!0.06  Базовый коэфф трения качения шара
        public const float VERTICAL_ROTATION_POWER = 0.07f; 	//Мощность влияния вертикального винта на траекторию
		public const float VERTICAL_ROTATION_DISSIPARATION = 5f;	//Скорость рассеивания вертикального винта (значение длины вектора, на который вектор винта сближается с вектором поступательного движения)
		public const float VERTICAL_ROTATION_WALL_ABSORB = 0.8f;	//Коэффициент поглощения вертикального винта при столкновении с бортом
		//public const float SIDE_ROTATION_POWER = 0.15f;		//Можность влияния бокового винта на траекторию отскока (длина вектора для корректировки) (доля от MAX_SHOT_POWER)
		public const float SIDE_ROTATION_DISSIPARATION = 0.95f;		//Скорость рассеивания бокового винта
        
        public const float MAX_VERTICAL_ROTATION_LEN = 120f;	//Максимальное значение придаваемого вертикального винта (при spiVx == 1 или -1)
		public const float MAX_SIDE_ROTATION_LEN    = 20f;      //Максимальное значение придаваемого бокового винта (при spiVx == 1 или -1)

        public const int headLineX			= -155;
		public const int leftBorderX 		= -310;
		public const int rightBorderX 		= 310;
		public const int topBorderY			= 155;
		public const int bottomBorderY		= -155;
		public const float cueBallPosX		= -180;
		public const float cueBallPosY		= 0;

        public const int POCKET_RAD_PX 			= 18;
		public const int POCKET_RAD_PX_SQUARED 	= 18 * 18;

        public const float BALL_COLLISION_POWER_MISS = 0.95f;	//Коэффициент потери энергии на столкновение шаров

        public const float WALL_ELASTIC 	= 0.75f; //Поглощение скорости бортами

        public const int COORD_ROUND_TO = 10;//Точность округления координат шаров (например до тысячных = 1000)
    }
}
