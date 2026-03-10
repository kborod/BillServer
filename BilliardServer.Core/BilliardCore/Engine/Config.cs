using Kborod.BilliardCore.Enums;
using System;
using System.Collections.Generic;

namespace Kborod.BilliardCore
{
    public class Config
    {
        public const float MODEL_COORD_TO_WORLD_KOEF = 0.0162f;

        // !!!250 максимальная длина вектора скорости шара
        public static readonly Fixed64 MAX_SHOT_POWER           = Fixed64.FromInt(300);

        public static readonly Fixed64 SPEED_UPDATE_DELTA = Fixed64.FromInt(100);    //через каждый SPEED_UPDATE_DELTA происходит корректировка векторов движения (трение, вращение и т.д.)
        public static readonly Fixed64 BALLS_INTEGRATE_DELTA = Fixed64.FromInt(10);	//через каждый BALLS_INTEGRATE_DELTA происходит интегрирование модели

        public static readonly Fixed64 BALL_DIAM_PX 			= Fixed64.FromInt(18);
		public static readonly Fixed64 BALL_DIAM_PX_SQUARED 	= Fixed64.FromInt(18 * 18);
        public static readonly Fixed64 BALL_RAD_PX              = Fixed64.FromInt(9);
		public static readonly Fixed64 BALL_RAD_PX_SQUARED		= Fixed64.FromInt(9 * 9);

        public static readonly Fixed64 BALL_FRICTION 		            = Fixed64.FromDouble(0.04);    //!!!0.06  Базовый коэфф трения качения шара
        public static readonly Fixed64 VERTICAL_ROTATION_POWER          = Fixed64.FromDouble(0.07); 	//Мощность влияния вертикального винта на траекторию
		public static readonly Fixed64 VERTICAL_ROTATION_DISSIPARATION  = Fixed64.FromDouble(5);	//Скорость рассеивания вертикального винта (значение длины вектора, на который вектор винта сближается с вектором поступательного движения)
		public static readonly Fixed64 VERTICAL_ROTATION_WALL_ABSORB    = Fixed64.FromDouble(0.8); //Коэффициент поглощения вертикального винта при столкновении с бортом
                                                                                                    //public const float SIDE_ROTATION_POWER = 0.15f;		//Можность влияния бокового винта на траекторию отскока (длина вектора для корректировки) (доля от MAX_SHOT_POWER)
        public static readonly Fixed64 SIDE_ROTATION_DISSIPARATION = Fixed64.FromDouble(0.95);		//Скорость рассеивания бокового винта
        
        public static readonly Fixed64 MAX_VERTICAL_ROTATION_LEN = Fixed64.FromDouble(120);	//Максимальное значение придаваемого вертикального винта (при spiVx == 1 или -1)
		public static readonly Fixed64 MAX_SIDE_ROTATION_LEN    = Fixed64.FromDouble(20);      //Максимальное значение придаваемого бокового винта (при spiVx == 1 или -1)

        public static readonly Fixed64 headLineX			= Fixed64.FromInt(-155);
		public static readonly Fixed64 leftBorderX 		    = Fixed64.FromInt(-310);
		public static readonly Fixed64 rightBorderX 		= Fixed64.FromInt(310);
		public static readonly Fixed64 topBorderY			= Fixed64.FromInt(155);
		public static readonly Fixed64 bottomBorderY		= Fixed64.FromInt(-155);
		public static readonly Fixed64 cueBallPosX		    = Fixed64.FromInt(-180);
		public static readonly Fixed64 cueBallPosY		    = Fixed64.FromInt(0);

        public static readonly Fixed64 POCKET_RAD_PX 			= Fixed64.FromInt(18);
        public static readonly Fixed64 POCKET_RAD_PX_SQUARED 	= Fixed64.FromInt(18 * 18);

        public static readonly Fixed64 BALL_COLLISION_POWER_MISS = Fixed64.FromDouble(0.95);	//Коэффициент потери энергии на столкновение шаров

        public static readonly Fixed64 WALL_ELASTIC 	= Fixed64.FromDouble(0.75); //Поглощение скорости бортами

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

            ((BallData)balls[0]).SetPosition(Config.cueBallPosX, Config.cueBallPosY);


            var fPosNum = Fixed64.FromInt(posNum);

            var x0 = Fixed64.FromDouble(158) + (fPosNum - Fixed64.Five);
            var y0 = Fixed64.Zero + (fPosNum - Fixed64.Five);


            balls[9].SetPosition(x0, y0);
            var step = Fixed64.Sqrt(Config.BALL_DIAM_PX_SQUARED - Config.BALL_RAD_PX_SQUARED) + Fixed64.FromDouble(0.1) + (Fixed64.FromDouble(0.5) * fPosNum / Fixed64.FromInt(10));
            balls[12].SetPosition(x0 + step, y0 - Config.BALL_RAD_PX);
            balls[7].SetPosition(x0 + step, y0 + Config.BALL_RAD_PX);
            balls[1].SetPosition(x0 + Fixed64.Two * step, y0 - Fixed64.Two * Config.BALL_RAD_PX);
            balls[8].SetPosition(x0 + Fixed64.Two * step, y0);
            balls[15].SetPosition(x0 + Fixed64.Two * step, y0 + Fixed64.Two * Config.BALL_RAD_PX);
            balls[14].SetPosition(x0 + Fixed64.Three * step, y0 - Fixed64.Three * Config.BALL_RAD_PX);
            balls[3].SetPosition(x0 + Fixed64.Three * step, y0 - Fixed64.One * Config.BALL_RAD_PX);
            balls[10].SetPosition(x0 + Fixed64.Three * step, y0 + Fixed64.One * Config.BALL_RAD_PX);
            balls[6].SetPosition(x0 + Fixed64.Three * step, y0 + Fixed64.Three * Config.BALL_RAD_PX);
            balls[5].SetPosition(x0 + Fixed64.Four * step, y0 - Fixed64.Four * Config.BALL_RAD_PX);
            balls[4].SetPosition(x0 + Fixed64.Four * step, y0 - Fixed64.Two * Config.BALL_RAD_PX);
            balls[13].SetPosition(x0 + Fixed64.Four * step, y0);
            balls[2].SetPosition(x0 + Fixed64.Four * step, y0 + Fixed64.Two * Config.BALL_RAD_PX);
            balls[11].SetPosition(x0 + Fixed64.Four * step, y0 + Fixed64.Four * Config.BALL_RAD_PX);

            return balls;
        }
    }
}
