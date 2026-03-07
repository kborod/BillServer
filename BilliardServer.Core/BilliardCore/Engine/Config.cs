using Kborod.BilliardCore.Enums;
using System;
using System.Collections.Generic;

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

        public static List<BallData> GetBallsPositionsForNewGame(GameType gameType, int posNum)
        {
            if (gameType != GameType.PoolEight)
                throw new NotImplementedException();

            var balls = new List<BallData>(16);
            for (var i = 0; i <= 15; i++)
            {
                balls.Add(new BallData { Number = i });
            }

            balls[0].SetPosition(Config.cueBallPosX, Config.cueBallPosY);

            var x0 = 158f + (posNum - 5);
            var y0 = 0f + (posNum - 5);

            balls[9].SetPosition(x0, y0);
            var step = MathF.Sqrt(Config.BALL_DIAM_PX_SQUARED - Config.BALL_RAD_PX_SQUARED) + 0.1f + (0.5f * posNum / 10f);
            balls[12].SetPosition(x0 + step, y0 - Config.BALL_RAD_PX);
            balls[7].SetPosition(x0 + step, y0 + Config.BALL_RAD_PX);
            balls[1].SetPosition(x0 + 2 * step, y0 - 2 * Config.BALL_RAD_PX);
            balls[8].SetPosition(x0 + 2 * step, y0);
            balls[15].SetPosition(x0 + 2 * step, y0 + 2 * Config.BALL_RAD_PX);
            balls[14].SetPosition(x0 + 3 * step, y0 - 3 * Config.BALL_RAD_PX);
            balls[3].SetPosition(x0 + 3 * step, y0 - 1 * Config.BALL_RAD_PX);
            balls[10].SetPosition(x0 + 3 * step, y0 + 1 * Config.BALL_RAD_PX);
            balls[6].SetPosition(x0 + 3 * step, y0 + 3 * Config.BALL_RAD_PX);
            balls[5].SetPosition(x0 + 4 * step, y0 - 4 * Config.BALL_RAD_PX);
            balls[4].SetPosition(x0 + 4 * step, y0 - 2 * Config.BALL_RAD_PX);
            balls[13].SetPosition(x0 + 4 * step, y0);
            balls[2].SetPosition(x0 + 4 * step, y0 + 2 * Config.BALL_RAD_PX);
            balls[11].SetPosition(x0 + 4 * step, y0 + 4 * Config.BALL_RAD_PX);

            return balls;
        }
    }
}
