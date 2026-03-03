using System;

namespace Kborod.BilliardCore
{
    public class Ball
    {
        public int Number { get; private set; }

        /// <summary> Признак, надо ли обновить состояние после итерации. </summary>
        public bool NeedUpdateState { get; set; } = false;

        /// <summary> Признак - находится ли шар в состоянии покоя </summary>
        public bool IsSleep { get; set; } = true;

        /// <summary> вектор поступательного движения шара </summary>
        public MyVector v { get; private set; } = new MyVector();

        /// <summary> Вектор вертикального вращения шара </summary>
        public MyVector vVertSpin { get; private set; } = new MyVector();
		
		/// <summary>
		/// Сила бокового вращения шара (длина вектора корректировки отскока)
		/// Если< 0, значит вращение во часовой стрелке (берем -dx и -dy стены)
		/// Если > 0, значит вращение против часовой стрелки(берем +dx и +dy стены)
		/// </summary>
		public float SideSpin { get; set; }




        //ШАР ЗАБИТ:
        /// <summary> Луза, в которую забит шар. </summary>
        public Pocket PocketRemoveTo { get; set; } = null;

        /// <summary> признак - находится ли шар на столе (или уже забит) </summary>
        public bool IsRemoved { get; set; } = false;

        /// <summary> Оставшееся неинтегрированное время тика после забития шара </summary>
        public float RemoveDeltaTime { get;  set; }  = 0;

        /// <summary> Координата Z (для скрытия шара под столом) </summary>
        public float Zcoordinate { get; private set; }




        private readonly float _friction;

        /// <summary> Инкремент трения при маленькой скорости шара (чтоб остановился быстрее) </summary>
        private float _lastFr;


        public Ball(int ballNumber)
        {
            Number = ballNumber;

            _friction = Config.BALL_FRICTION;
            ResetParams();
        }

        public void SetPosition(float posX, float posY)
		{
			v.p0.x = posX;
			v.p0.y = posY;
			v.updatePointsFromComponents();
		}

		/**
		 * Произвести удар по шару (установить параметры вращения и поступательного движения)
		 * @param	vx
		 * @param	vy
		 * @param	spinVx от -1 до 1 (боковое вращение)
		 * @param	spinVy от -1 до 1 (вертикальное вращение)
		 */
		public void MakeShot(float vx, float vy, float spinVx, float spinVy)
		{
			v.vx = vx;
			v.vy = vy;
			v.updatePointsFromComponents();
			v.makeVector();
			
			vVertSpin.vx = v.dx* spinVy * Config.MAX_VERTICAL_ROTATION_LEN * (v.len / 150);
			vVertSpin.vy = v.dy* spinVy * Config.MAX_VERTICAL_ROTATION_LEN * (v.len / 150);
			vVertSpin.updatePointsFromComponents();
			vVertSpin.makeVector();

			SideSpin = spinVx * (Config.MAX_SIDE_ROTATION_LEN / 2 + (Config.MAX_SIDE_ROTATION_LEN / 2) * (v.len / 150)); // * Config.SIDE_ROTATION_POWER * v.len;
			//trace("ASDSD", sideSpin, spinVx, v.len);
		}

		public void Integrate(float timeKoef)
		{
			if (IsRemoved) return;
			v.p0.x += v.vx * timeKoef;
			v.p0.y += v.vy * timeKoef;
			v.updatePointsFromComponents();
			v.updateLen();
		}
		
		public void UpdateVelocities()
		{
			//расчитываем коэфф трения (чем больше скорость шара, тем он меньше)
			var speedfrKoef = (0.2f + (1 - 0.2f) * (1 - (v.len / Config.MAX_SHOT_POWER)));
			if (Number == 0)
			{
				//trace ("speedfrKoef:" + speedfrKoef);
				//trace ("  v.vx:" + v.vx + ";v.vy:"+ v.vy);
				//trace ("  v.len:" + v.len);
				//trace ("  vVertSpin.vx:" + vVertSpin.vx + ";vVertSpin.vy:"+ vVertSpin.vy);
				//trace ("  vSummSpeed.len:" + vSummSpeed.len);
			}

			if (v.len < 3)
			{
				_lastFr = 0.17f * (1 - v.len * v.len / 9);
			}
			else
			{
				_lastFr = 0;
			}
			//trace (lastFr);
			//trace ("   " + v.len);

			v.vx = v.vx * (1 - _friction * speedfrKoef - _lastFr);
			v.vy = v.vy * (1 - _friction * speedfrKoef - _lastFr);

			UpdateVerticalRotation(speedfrKoef);
			UpdateSideRotation(speedfrKoef);

			if (v.len < 0.25 && vVertSpin.len < 0.25 && (SideSpin > -0.25 && SideSpin < 0.25))
			{
				StopBall();

				//GameCore.GCLink.updateActiveState(this);
				NeedUpdateState = true;
			}

			v.updatePointsFromComponents();
			v.makeVector();
		}
		
		
		private void UpdateVerticalRotation(float speedFrKoef)
		{
            float len;
            MyVector vc = new MyVector();
            //var vm:MyVector = new MyVector;	//Вектор = сумме векторов поступательного движения и вертикального винта
            float k;
            float k2;  //Коэффициент корректировки силы трения (тем выше, чем меньше скорость в точке опоры шара)

            //Расстояние между p1 вектора v и p1 вектора vVertSpin. (если эти точки расположены рядом, 
            //то вектор поступательного движения и вектор вертикального вращения совпадают - шар катится а не скользит)
            len = MathF.Sqrt( (v.vx - vVertSpin.vx) * (v.vx - vVertSpin.vx) + (v.vy - vVertSpin.vy) * (v.vy - vVertSpin.vy) ) ;
			//if (bNumber == 0) trace("***" + v.len + " " + len);
			//trace ("**" + v.vx + "-----" + v.vy + "     " + len);
			if (len < 1)
			{
				vVertSpin.vx = v.vx;
				vVertSpin.vy = v.vy;
			}
			else
			{
				/*vm.vx = v.vx + vVertSpin.vx;
				vm.vy = v.vy + vVertSpin.vy;
				vm.updatePointsFromComponents();
				vm.makeVector();*/
				/*if (vm.len < 50)
				{
					k2 = 1 + (2.5 - 2.5 * vm.len / 50);
				}
				else
				{
					k2 = 1;
				}*/
				if (len < 50)
				{
					k2 = 1 + (2 - 2 * len / 50);
				}
				else
				{
					k2 = 1;
				}
				//trace(len, k2);
				v.vx = v.vx + (-v.vx + vVertSpin.vx) * Config.VERTICAL_ROTATION_POWER * speedFrKoef * k2;
				v.vy = v.vy + (-v.vy + vVertSpin.vy) * Config.VERTICAL_ROTATION_POWER * speedFrKoef * k2;
				vc.vx = (v.vx - vVertSpin.vx);
				vc.vy = (v.vy - vVertSpin.vy);
				vc.updatePointsFromComponents();
				vc.makeVector();
				k = Config.VERTICAL_ROTATION_DISSIPARATION * k2;
				if (k > len) k = len;
				vVertSpin.vx = vVertSpin.vx + (vc.dx * k);
				vVertSpin.vy = vVertSpin.vy + (vc.dy * k);
			}

			vVertSpin.updatePointsFromComponents();
			vVertSpin.makeVector();
		}
		
		private void UpdateSideRotation(float speedFrKoef)
		{
			//Инкремент трения при маленькой скорости шара (чтоб остановился быстрее)
			if (v.len < 3 && SideSpin > -2 && SideSpin < 2)
			{
				_lastFr = 0.2f * (1 - v.len  / 3);
			}
            else
			{
				_lastFr = 0;
			}
			SideSpin *= 0.95f + (0.05f * (1 - speedFrKoef)) - _lastFr;
			/*if (bNumber == 0)
			{
				trace (sideSpin);
				//trace ("   " + String(0.95 + (0.05 * (1 - speedFrKoef))  - lastFr));
				//trace ("   " + String(0.95 - lastFr));
				//trace ("   " + v.len);
			}*/
		}

        /// <summary>
        /// Переместить спрайти на верхний слой
        /// </summary>
        private void MoveToUpperLayer()
		{
			Zcoordinate = 0;
		}
        /// <summary>
        /// Переместить спрайти на нижний слой
        /// </summary>
        /// <returns></returns>
        public void MoveToBottomLayer()
		{
			Zcoordinate = 1.5f;
		}
		
		public void StopBall()
		{
			v.vx = 0;
			v.vy = 0;
			v.updatePointsFromComponents();
			v.makeVector();

			vVertSpin.vx = 0;
			vVertSpin.vy = 0;
			vVertSpin.updatePointsFromComponents();
			vVertSpin.makeVector();

			SideSpin = 0;
		}
		
		public void ResetParams()
		{
			NeedUpdateState = false;
			IsSleep = true;
			PocketRemoveTo = null;
			IsRemoved = false;
			MoveToUpperLayer();
		}

		public void ChangeNumber(int number)
		{
			Number = number;
		}
	}
}
