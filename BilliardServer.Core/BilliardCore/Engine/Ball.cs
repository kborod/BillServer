using System;

namespace Kborod.BilliardCore
{
    public class Ball : IBallData
    {
        public int Number { get; private set; }

        /// <summary> признак - находится ли шар на столе (или уже забит) </summary>
        public bool IsRemoved { get; set; } = false;

        public long Xraw => X.Raw;

        public long Yraw => Y.Raw;

        public Fixed64 X => v.p0.x;

        public Fixed64 Y => v.p0.y;



        /// <summary> Признак, надо ли обновить состояние после итерации. </summary>
        public bool NeedUpdateState { get; set; }

        /// <summary> Признак - находится ли шар в состоянии покоя </summary>
        public bool IsSleep { get; set; }

        /// <summary> вектор поступательного движения шара </summary>
        public MyVector v { get; private set; }

        /// <summary> Вектор вертикального вращения шара </summary>
        public MyVector vVertSpin { get; private set; }

        /// <summary>
        /// Сила бокового вращения шара (длина вектора корректировки отскока)
        /// Если< 0, значит вращение во часовой стрелке (берем -dx и -dy стены)
        /// Если > 0, значит вращение против часовой стрелки(берем +dx и +dy стены)
        /// </summary>
        public Fixed64 SideSpin { get; set; }




        //ШАР ЗАБИТ:
        /// <summary> Луза, в которую забит шар. </summary>
        public Pocket PocketRemoveTo { get; set; }

        /// <summary> Оставшееся неинтегрированное время тика после забития шара </summary>
        public Fixed64 RemoveDeltaTime { get; set; }

        /// <summary> Координата Z (для скрытия шара под столом) </summary>
        public Fixed64 Zcoordinate { get; private set; }




        private readonly Fixed64 _friction;

        /// <summary> Инкремент трения при маленькой скорости шара (чтоб остановился быстрее) </summary>
        private Fixed64 _lastFr;


        public Ball(int ballNumber)
        {
            Number = ballNumber;

            _friction = Config.BALL_FRICTION;
            ResetParams();
        }

        public void SetPosition(Fixed64 posX, Fixed64 posY)
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
        public void MakeShot(Fixed64 vx, Fixed64 vy, Fixed64 spinVx, Fixed64 spinVy)
        {
            v.vx = vx;
            v.vy = vy;
            v.updatePointsFromComponents();
            v.makeVector();

            vVertSpin.vx = v.dx * spinVy * Config.MAX_VERTICAL_ROTATION_LEN * (v.len / Fixed64.FromInt(150));
            vVertSpin.vy = v.dy * spinVy * Config.MAX_VERTICAL_ROTATION_LEN * (v.len / Fixed64.FromInt(150));
            vVertSpin.updatePointsFromComponents();
            vVertSpin.makeVector();

            SideSpin = spinVx * (Config.MAX_SIDE_ROTATION_LEN / Fixed64.FromInt(2) + (Config.MAX_SIDE_ROTATION_LEN / Fixed64.FromInt(2)) * (v.len / Fixed64.FromInt(150))); // * Config.SIDE_ROTATION_POWER * v.len;
                                                                                                                                                                            //trace("ASDSD", sideSpin, spinVx, v.len);
        }

        public void Integrate(Fixed64 timeKoef)
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
            var kBase = Fixed64.FromDouble(0.2);
            var speedfrKoef = (kBase + (Fixed64.One - kBase) * (Fixed64.One - (v.len / Config.MAX_SHOT_POWER)));
            if (Number == 0)
            {
                //trace ("speedfrKoef:" + speedfrKoef);
                //trace ("  v.vx:" + v.vx + ";v.vy:"+ v.vy);
                //trace ("  v.len:" + v.len);
                //trace ("  vVertSpin.vx:" + vVertSpin.vx + ";vVertSpin.vy:"+ vVertSpin.vy);
                //trace ("  vSummSpeed.len:" + vSummSpeed.len);
            }

            if (v.len < Fixed64.FromInt(3))
            {
                _lastFr = Fixed64.FromDouble(0.17d) * (Fixed64.One - v.len * v.len / Fixed64.FromInt(9));
            }
            else
            {
                _lastFr = Fixed64.Zero;
            }
            //trace (lastFr);
            //trace ("   " + v.len);

            v.vx = v.vx * (Fixed64.One - _friction * speedfrKoef - _lastFr);
            v.vy = v.vy * (Fixed64.One - _friction * speedfrKoef - _lastFr);

            UpdateVerticalRotation(speedfrKoef);
            UpdateSideRotation(speedfrKoef);

            var threshold = Fixed64.FromDouble(0.25);
            if (v.len < threshold && vVertSpin.len < threshold && (SideSpin > -threshold && SideSpin < threshold))
            {
                StopBall();

                //GameCore.GCLink.updateActiveState(this);
                NeedUpdateState = true;
            }

            v.updatePointsFromComponents();
            v.makeVector();
        }


        private void UpdateVerticalRotation(Fixed64 speedFrKoef)
        {
            Fixed64 len;
            MyVector vc = new MyVector();
            //var vm:MyVector = new MyVector;	//Вектор = сумме векторов поступательного движения и вертикального винта
            Fixed64 k;
            Fixed64 k2;  //Коэффициент корректировки силы трения (тем выше, чем меньше скорость в точке опоры шара)

            //Расстояние между p1 вектора v и p1 вектора vVertSpin. (если эти точки расположены рядом, 
            //то вектор поступательного движения и вектор вертикального вращения совпадают - шар катится а не скользит)
            len = Fixed64.Sqrt((v.vx - vVertSpin.vx) * (v.vx - vVertSpin.vx) + (v.vy - vVertSpin.vy) * (v.vy - vVertSpin.vy));
            //if (bNumber == 0) trace("***" + v.len + " " + len);
            //trace ("**" + v.vx + "-----" + v.vy + "     " + len);
            if (len < Fixed64.One)
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
                if (len < Fixed64.FromInt(50))
                {
                    k2 = Fixed64.One + (Fixed64.Two - Fixed64.Two * len / Fixed64.FromInt(50));
                }
                else
                {
                    k2 = Fixed64.One;
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

        private void UpdateSideRotation(Fixed64 speedFrKoef)
        {
            //Инкремент трения при маленькой скорости шара (чтоб остановился быстрее)
            if (v.len < Fixed64.FromInt(3) && SideSpin > -Fixed64.Two && SideSpin < Fixed64.Two)
            {
                _lastFr = Fixed64.FromDouble(0.2) * (Fixed64.One - v.len / Fixed64.FromInt(3));
            }
            else
            {
                _lastFr = Fixed64.Zero;
            }
            SideSpin *= Fixed64.FromDouble(0.95) + (Fixed64.FromDouble(0.05) * (Fixed64.One - speedFrKoef)) - _lastFr;
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
            Zcoordinate = Fixed64.Zero;
        }
        /// <summary>
        /// Переместить спрайти на нижний слой
        /// </summary>
        /// <returns></returns>
        public void MoveToBottomLayer()
        {
            Zcoordinate = Fixed64.FromDouble(1.5);
        }

        public void StopBall()
        {
            v.vx = Fixed64.Zero;
            v.vy = Fixed64.Zero;
            v.updatePointsFromComponents();
            v.makeVector();

            vVertSpin.vx = Fixed64.Zero;
            vVertSpin.vy = Fixed64.Zero;
            vVertSpin.updatePointsFromComponents();
            vVertSpin.makeVector();

            SideSpin = Fixed64.Zero;
        }

        public void ResetParams()
        {
            NeedUpdateState = false;
            IsSleep = true;
            PocketRemoveTo = null;
            RemoveDeltaTime = Fixed64.Zero;
            IsRemoved = false;
            v = new MyVector();
            vVertSpin = new MyVector();
            SideSpin = Fixed64.Zero;
            MoveToUpperLayer();
        }

        public void ChangeNumber(int number)
        {
            Number = number;
        }
    }
}
