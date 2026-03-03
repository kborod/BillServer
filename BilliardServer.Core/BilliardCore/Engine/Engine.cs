using System;
using System.Collections.Generic;

namespace Kborod.BilliardCore
{
    // TODO Оптимизация расчета времени следующего столкновения - 4 вектора бортов. Шары сначала с ними проверять.
    // В зависимости от u1 и u2 определять, надо ли проверять с угловыми бортами и лузами. В большинстве случаев будет достаточно проверки шара с этими 4-я векторами

    public class Engine : IEngineForUI
    {
        private ShotResult shotCalculateResult;

		public List<Ball> Balls { get; private set; } = new List<Ball>();
		public List<MyVector> RealWalls { get; private set; } = new List<MyVector>(); 	//реальные координаты стен 
		private List<MyVector> walls = new List<MyVector>();		//стены, смещенные на радиус шара
		private List<Angle> angles = new List<Angle>(); 		//углы (шары с центром в углах)
		private List<Pocket> pockets = new List<Pocket>();		//Лузы
		
		private float currShotDuration = 0;     //время текущего текущего удара
		private float timeToVelUpdate = 0;		//время до следующего обновления скоростей
		
		private int cueBallNum = -1;

		private int MatchShotsCount = 0;		//Всего ударов в матче
		
		public Engine()
		{ 
			createWalls();
			
			createPockets();

			//LogCoords();
		}

        private void LogCoords()
        {
			//var deltaX = -380;
			//var deltaY = -275;// -262.275;

   //         List<Point[]> coords = new List<Point[]>();
			//foreach (var item in RealWalls)
			//{
			//	coords.Add(new Point[] { new Point(item.p0.x, item.p0.y), new Point(item.p1.x, item.p1.y) });
			//}
			//var s = "Walls:\n";
   //         foreach (var pair in coords)
   //         {
			//	//s += $"[{pair[0].x + deltaX};{pair[0].y + deltaY}] [{pair[1].x + deltaX};{pair[1].y + deltaY}]\n";
			//	s += string.Format("{{new Point[]{{new Point({0}, {1}), new Point({2}, {3})}}}},\n", 
			//		$"{(pair[0].x + deltaX).ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"))}f",
   //                 $"{(pair[0].y + deltaY).ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"))}f",
			//		$"{(pair[1].x + deltaX).ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"))}f",
			//		$"{(pair[1].y + deltaY).ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"))}f"
   //                 );
   //         }
			//Debug.Log(s);


			//coords.Clear();
   //         foreach (var item in pockets)
   //         {
   //             coords.Add(new Point[] { new Point(item.x, item.y), new Point(item.vRemove.vx, item.vRemove.vy) });
   //         }
   //         s = "Pockets:\n"; 
			//foreach (var pair in coords)
   //         {
			//	//s += $"[{pair[0].x + deltaX};{pair[0].y + deltaY}] [{pair[1].x};{pair[1].y}]\n";
			//	s += string.Format("{{new Point[]{{new Point({0},{1}), new Point({2},{3})}}}},\n",
   //                 $"{(pair[0].x + deltaX).ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"))}f",
			//		$"{(pair[0].y + deltaY).ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"))}f",
			//		$"{(pair[1].x).ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"))}f",
   //                 $"{(pair[1].y).ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"))}f"
   //                 );
   //         }
   //         Debug.Log(s);
        }

        /**
		 * Установить координаты шара без проверок
		 */
        public void SetBallPosition(int ballNum, float posX, float posY)
		{
			Balls[ballNum].SetPosition(posX, posY);
		}



        /// <summary>
		/// Переместить шар с проверкой. Если новая координата за пределами стола или дома, то перемещается внутрь стола.
		/// Если шар соприкосается с другим шаром, то возвращает false и оставляет старые координаты. Иначе устанавливает
		/// новые координаты и возвращает true. 
        /// </summary>
        /// <returns>Если позиция не корректна, то возвращает false, иначе true</returns>
        public bool ReplaceBall(int ballNum, float posX, float posY, bool onlyKitchen, bool correctionAllowed)
		{
			// TODO Добавить алгоритм выталкивания шара из кучи других шаров
			var tmpX = posX;
			var tmpY = posY;
			if (posX < Config.leftBorderX + Config.BALL_RAD_PX)
			{
				if (!correctionAllowed) return false;
				tmpX = Config.leftBorderX + Config.BALL_RAD_PX + 0.01f;
			}
			if (onlyKitchen)
			{
				if (posX > Config.headLineX - Config.BALL_RAD_PX)
				{
                    if (!correctionAllowed) return false;
                    tmpX = Config.headLineX - Config.BALL_RAD_PX - 0.01f;
				}
			}
			else
			{
				if (posX > Config.rightBorderX - Config.BALL_RAD_PX)
                {
                    if (!correctionAllowed) return false;
                    tmpX = Config.rightBorderX - Config.BALL_RAD_PX - 0.01f; 
				}
			}
			if (posY > Config.topBorderY - Config.BALL_RAD_PX)
			{
                if (!correctionAllowed) return false;
                tmpY = Config.topBorderY - Config.BALL_RAD_PX - 0.01f;
			}
			if (posY < Config.bottomBorderY + Config.BALL_RAD_PX)
            {
                if (!correctionAllowed) return false;
                tmpY = Config.bottomBorderY + Config.BALL_RAD_PX + 0.01f;
			}

			foreach (var ball in Balls)
			{
				if (ball.Number == ballNum || ball.IsRemoved == true) continue;
				vc.p0.x = tmpX;
				vc.p0.y = tmpY;
				vc.p1.x = ball.v.p0.x;
				vc.p1.y = ball.v.p0.y;
				vc.updateComponentsFromPoints();
				vc.makeVector();

				if (Config.BALL_DIAM_PX - vc.len >= -0.01)
                    return false;
            }
			SetBallPosition(ballNum, tmpX, tmpY);
			return true;
		}

        /// <summary>
        /// Вернуть на стол забитый шар (меняются координаты шара)
        /// </summary>
        /// <param name="ballNum"></param>
        public void ReturnPocketedBall(int ballNum)
		{
			// TODO Доработать алгоритм подбора позиции
			if ((Balls[ballNum] as Ball).IsRemoved == false)
			{
				ToLogError("ERROR returnPocketedBall(). ball.isRemoved = false. ballNum:" + ballNum);
				return;
			}
			var ball = Balls[ballNum] as Ball;

            ball.StopBall();
			ball.ResetParams();
            //ball.isRemoved = false;
            //ball.isSleep = true;
            //ball.moveToUpperLayer();

			var tmpX = Config.cueBallPosX;
			var tmpY = Config.cueBallPosY;
			while (true)
			{
				if (ReplaceBall(ballNum, tmpX, tmpY, onlyKitchen: false, correctionAllowed: true) == true)
				{
					return;
				}
				tmpX += Config.BALL_RAD_PX;
			}
		}


        /// <summary> Установить компоненты скорости шару (произвести удар по шару) </summary>
        public void MakeShot(float vx, float vy, int ballNumber, float spinVx, float spinVy)
		{
			if (activeBalls.Count > 0 && currShotDuration > 0)
			{
                ToLogError("ERROR makeShot(): shot under process");
				return;
			}
			else if (activeBalls.Count == 0)
			{
                resetShotParams();
                shotCalculateResult = new ShotResult();
            }

            if (Balls.Count < ballNumber || Balls[ballNumber] == null)
			{
                ToLogError("ERROR makeShot():" + ballNumber + " undefined");
				return;
			}
			else
			{
				this.cueBallNum = ballNumber;
				
				Balls[ballNumber].MakeShot(vx, vy, spinVx, spinVy);
				
				addActiveBall(Balls[ballNumber]);
				
				UpdateNextCollisionTime();
			}

			MatchShotsCount++;
		}
		
		private float currDt = 0;			//значение дельта, на которое обновляем модель
		private float lastDt = 0;			//Значение дельта, оставшееся с предыдущей итерации
		
		
		/**
		 * Просчитать состояние модели шаров через deltaTime
		 * Возвращает Null, если есть активные шары и модель в процессе удара
		 * Или возвращает ShotCalculateResult, если удар завершен.
		 * @param	dt
		 * дельта времени
		 */
		public void UpdateModel(int deltaTime, out ShotTickResult tickResult, out ShotResult shotResultOrNull)
		{
			if (deltaTime <= 0) { ToLogError("ERROR updateModel. deltaTime:" + deltaTime); }

            tickResult = new ShotTickResult();
			tickResult.SetDeltaTime(deltaTime);
            shotResultOrNull = null;

            currDt = deltaTime + lastDt;

            //выполнять, пока не будут просчитаны все столкновения за deltatime
            while (true)
			{
				if (activeBalls.Count == 0) 
				{					
					shotCalculateResult.ShotDuration = currShotDuration;
					shotCalculateResult.MatchShotsCount = MatchShotsCount;
					roundBallsCoord();
					shotResultOrNull = shotCalculateResult;
                    return;
				}
				
				var needUpdateCollTime = false;
				var minValueTmp = (float) Config.BALLS_INTEGRATE_DELTA;

				if (minValueTmp > currDt) { minValueTmp = currDt; }
				if (minValueTmp > collisions.TimeToCollision) { minValueTmp = collisions.TimeToCollision; }
				if (minValueTmp > timeToVelUpdate) { minValueTmp = timeToVelUpdate; }
				
				if 	(minValueTmp == Config.BALLS_INTEGRATE_DELTA)
				{
					IntegrateModel(minValueTmp);
					
					if (minValueTmp == collisions.TimeToCollision)
					{
						ProcessCollisions(ref tickResult); 	
						needUpdateCollTime = true;
					}
					else if (minValueTmp == timeToVelUpdate)		
					{ 
						UpdateBallsVelocities(); 
						needUpdateCollTime = true;
					}
				}
				else if (minValueTmp == currDt)
				{
					lastDt = currDt;
					break;
				}
				else if (minValueTmp == collisions.TimeToCollision)
				{
					IntegrateModel(minValueTmp);
					ProcessCollisions(ref tickResult);
					if (minValueTmp == timeToVelUpdate) { UpdateBallsVelocities(); }
					needUpdateCollTime = true;
				}
				else if (minValueTmp == timeToVelUpdate)
				{
					IntegrateModel(minValueTmp);
					UpdateBallsVelocities();
					needUpdateCollTime = true;
				}
				
				if (needUpdateCollTime)
				{
					UpdateAllBallsState();
					UpdateNextCollisionTime();
				}
				
			}
		}
		
		private float tKoef;
		/**
		 * Интегрировать состояние модели на deltaTime без просчета столкновений (только переместить шары).
		 * @param	deltaTime
		 */
		private void IntegrateModel(float deltaTime)
		{
			tKoef = deltaTime / Config.SPEED_UPDATE_DELTA;
			
			for (var i = 0; i < Balls.Count; i++) 
			{
				Balls[i].Integrate(tKoef);
			}
			
			currDt -= deltaTime;
			timeToVelUpdate -= deltaTime;
			currShotDuration += deltaTime;
			collisions.TimeToCollision -= deltaTime;
			//toLog(currShotDuration + ": Model Integrated");
		}

		private Collisions collisions = new Collisions();
		
		
		/**
		 * Обновить время до следующего столкновения в модели - поле timeToNextColl.
		 */
		private void UpdateNextCollisionTime()
		{
			//toLog("balls[0]:" + (balls[0] as Ball).needUpdateState + "," + (balls[0] as Ball).v.p0.x + "," + (balls[0] as Ball).v.p0.y);
			collisions.Clear();
			
			for (var i = 0; i < activeBalls.Count; i++)
			{
				for (var j = i+1; j < activeBalls.Count; j++) 
				{
					check2BallsCollisionTime(activeBalls[i], activeBalls[j]);
				}
				
				for (var k = 0; k < Balls.Count; k++)
				{
					if (Balls[k].IsRemoved == true) continue;
					
					if (activeBalls[i].Number == k || Balls[k].IsSleep == false)
					{
						continue;
					}
					
					check2BallsCollisionTime(activeBalls[i], Balls[k]);
				}
				
				
				checkBallWithWallsCollisionTime(activeBalls[i]);
				
				checkBallWithAnglesCollisionTime(activeBalls[i]);
				
				checkBallWithPocketsCollisionTime(activeBalls[i]);
			}
			
			//toLog("TimeToNextColl Updated. currShotDuration:" + currShotDuration + ";collObj:" + collisionObj.getString());
		}		
		
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////---------------------------------ШАРЫ с ШАРАМИ---------------------------------///////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		
		/**
		 * Проверить, есть ли столкновение между шарами. Если есть, то обновить timeToNextColl и collisionObj
		 * @param	b1 шар1
		 * @param	b2 шар2
		 */
		private void check2BallsCollisionTime(Ball b1, Ball b2)
		{
			//trace ("check " + b1Tmp.bNumber + " " + b2Tmp.bNumber);
			var timeToNextColl = getTimeToNextBallsCollision(b1, b2);
			
			//проверяем, если время до столкновения меньше текущего, то обновляем.
			if (timeToNextColl < float.MaxValue && timeToNextColl <= collisions.TimeToCollision) 
			{
				collisions.Add2BallsCollision(b1, b2, timeToNextColl);
			}
		}
		
		private MyVector v1;
		private MyVector v2;
		private MyVector vc = new MyVector();
		private Bounce2BallsResult newv2Balls;
		private BounceBallResult newv1Ball;
		private MyVector v3;
		private ProjectResult vp;
		private MyVector vn = new MyVector();
		private float diff;
		private float moveBack;
		private MyVector v4 = new MyVector();
		
		/**
		 * Возвращает время в миллисекундах, через которое состоится столкновение b1 и b2. Если не столкнутся, то возвращает  Number.MAX_VALUE
		 * @param	b1
		 * @param	b2
		 * @return
		 */
		private float getTimeToNextBallsCollision(Ball b1, Ball b2)
		{
			v1 = b1.v; 	//TODO можно будет сразу на входе в параметрах принимать вектора, а не шары.
			v2 = b2.v;
			
			//vector between center points of ball
			vc.p0 = v1.p0;
			vc.p1 = v2.p0;
			vc.updateComponentsFromPoints();
			vc.makeVector();
			
			//sum of radius with balls distance
		
			var pen = Config.BALL_DIAM_PX  - vc.len;
			
			//check if balls collide at start
			if (pen >= 0.001)
			{
				ToLog(/*GameTime.getInstance().getServerTime() + " " + */ currDt + " ERROR getTimeToNextBallsCollision: collide at start. b1:" + b1.Number + " b2:" + b2.Number + " pen:" + pen); 
			}
			//reduce movement vector from ball2 from movement vector of ball1
			v3 = new MyVector();
			v3.p0 = v1.p0;
			v3.vx = v1.vx - v2.vx;
			v3.vy = v1.vy - v2.vy;
			v3.updatePointsFromComponents();
			v3.makeVector();
			
			//use v3 as new movement vector for collision calculation
			//projection of vc on v3
			vp = MyVector.projectVector(vc, v3.dx, v3.dy);
			
			//vector to center of ball2 in direction of movement vectors normal
			vn.p0 = new Point(v1.p0.x + vp.vx, v1.p0.y + vp.vy);
			vn.p1 = v2.p0;
			vn.updateComponentsFromPoints();
			vn.makeVector();
			
			//check if vn is shorter then combined radiuses
			diff = ( Config.BALL_DIAM_PX ) - vn.len;
			
			if (diff > 0 && (v3.vx * vp.vx + v3.vy * vp.vy) >= 0)
			{
				//collision
				//amount to move back moving ball
				moveBack = MathF.Sqrt(Config.BALL_DIAM_PX_SQUARED - vn.len * vn.len);
				
				//vector from ball1 starting point to its coordinates when collision happens
				v4.p0 = v1.p0;
				v4.p1 = new Point(vn.p0.x - moveBack * v3.dx, vn.p0.y - moveBack * v3.dy);
				v4.updateComponentsFromPoints();
				v4.makeVector();
				
				return (v4.len / v3.len) * Config.SPEED_UPDATE_DELTA;
			}
			return float.MaxValue;
		}
		
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////---------------------------------ШАРЫ с БОРТАМИ---------------------------------///////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
				
		/**
		 * Проверить, есть ли столкновение шара и бортов. Если есть, то обновить timeToNextColl и collisionObj
		 * @param	b шар
		 */

        private void checkBallWithWallsCollisionTime(Ball b)
		{
			for (var i = 0; i < walls.Count; i++) 
			{
				var timeToNextColl = getTimeToBallWithWallCollision(b, walls[i]);
			
				//проверяем, если время до столкновения меньше текущего, то обновляем.
				if (timeToNextColl < float.MaxValue && timeToNextColl <= collisions.TimeToCollision) 
				{ 
					collisions.AddBallWithWallCollision(b, walls[i], timeToNextColl);
				}
			}
		}
		
		
		
		private float getTimeToBallWithWallCollision(Ball b, MyVector w, bool isAim = false)
		{
            BallWallIntersectResult ir = getBallWallIntersectionInfo(b.v.p0.x, b.v.p0.y, b.v.p1.x, b.v.p1.y, w.p0.x, w.p0.y, w.p1.x, w.p1.y);
			
			if (ir.IsIntersect == false || !ir.IsNeedProcessCollision)
			{
				return float.MaxValue;
			}
			else
			{
				//todo для теста
				/*trace("BALLWithWallInfo. bNum:" + b.bNumber + " wall:" + w.p0.x + "," + w.p0.y + " bP0X:" + b.v.p0.x + " bP0Y:" + b.v.p0.y + " bP1X" + b.v.p1.x + " bP1Y" + b.v.p1.y + " bVx:" + b.v.vx + " bVy:" + b.v.vy 
						+ " IRU1:" + ir.u1 + " IRU2:" + ir.u2 + " IRVectProd:" + ir.vectorProduct + " IRIsIntersect:" + ir.isIntersect + " IRX:" + ir.x + " IRY:" + ir.y);
				*/
				return ir.u1 * Config.SPEED_UPDATE_DELTA;
			}
		}
		
		private BallWallIntersectResult ballWallIntersectResult = new BallWallIntersectResult();

		private BallWallIntersectResult getBallWallIntersectionInfo(float ballX1, float ballY1, float ballX2, float ballY2, 
			float wallX1, float wallY1, float walllX2, float wallY2)
		{
			ballWallIntersectResult.clear();
			
			var denumenator = (wallY2 - wallY1) * ( ballX2 - ballX1) - (walllX2 - wallX1) * (ballY2 - ballY1);
			
			//если прямые не параллельны:
			if (denumenator != 0)
			{
				ballWallIntersectResult.u1 = (((walllX2 - wallX1) * (ballY1 - wallY1)) - ((wallY2 - wallY1) * (ballX1 - wallX1))) / denumenator;
				ballWallIntersectResult.u2 = (((ballX2 - ballX1) * (ballY1 - wallY1)) - ((ballY2 - ballY1) * (ballX1 - wallX1))) / denumenator;
				ballWallIntersectResult.x = ballX1 + ballWallIntersectResult.u1 * (ballX2 - ballX1);
				ballWallIntersectResult.y = ballY1 + ballWallIntersectResult.u1 * (ballY2 - ballY1);

                //если P2 шара слева от вектора стены (z координата векторного произведения < 0), значит коллизия еще не обработана, иначе коллизия уже обработана и шар уже движется в направлении от стены
                ballWallIntersectResult.vectorProductZcoord = ((ballX2 - ballX1) * (wallY2 - wallY1) - (ballY2 - ballY1) * (walllX2 - wallX1));
			}

			return ballWallIntersectResult;
		}
		
		

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////---------------------------------ШАРЫ с УГЛАМИ---------------------------------///////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		/**
		 * Проверить, есть ли столкновение шара и углов. Если есть, то обновить timeToNextColl и collisionObj
		 * @param	b шар
		 */
		private void checkBallWithAnglesCollisionTime(Ball b)
		{
			for (var i = 0; i < angles.Count; i++) 
			{
				var timeToNextColl = getTimeToNextBallAngleCollision(b, angles[i]);
			
				//проверяем, если время до столкновения меньше текущего, то обновляем.
				if (timeToNextColl < float.MaxValue && timeToNextColl <= collisions.TimeToCollision) 
				{
					collisions.AddBallWithAngleCollision(b, angles[i], timeToNextColl);
				}
			}
		}


		/**
		 * Возвращает время в миллисекундах, через которое шар столкнотся с углом. Если не столкнутся, то возвращает  Number.MAX_VALUE
		 * @param	b1
		 * @param	b2
		 * @return
		 */
		private float getTimeToNextBallAngleCollision(Ball b, Angle a)
		{
			v1 = b.v; 	//TODO можно будет сразу на входе в параметрах принимать вектора, а не шары.
			v1.makeVector();
			
			//vector between center points of ball
			vc.p0 = v1.p0;
			vc.p1.x = a.x;
			vc.p1.y = a.y;
			vc.updateComponentsFromPoints();
			vc.makeVector();
			
			//sum of radius with balls distance
		
			var pen = Config.BALL_RAD_PX - vc.len;
			
			//check if balls collide at start
			if (pen >= 0.001)
			{
				ToLog(/*GameTime.getInstance().getServerTime() + " " + int(currDt) + */" ERROR getTimeToNextBallAngleCollision: collide at start. b:" + b.Number + " a:" + a.x + "," + a.y + " pen:" + pen); 
			}
			
			//projection of vc on v1
			vp = MyVector.projectVector(vc, v1.dx, v1.dy);
			
			//vector to center of angle in direction of movement vectors normal
			vn.p0 = new Point(v1.p0.x + vp.vx, v1.p0.y + vp.vy);
			vn.p1.x = a.x;
			vn.p1.y = a.y;
			vn.updateComponentsFromPoints();
			vn.makeVector();
			
			//check if vn is shorter then combined radiuses
			diff = ( Config.BALL_RAD_PX ) - vn.len;
			
			if (diff >= 0 && (v1.vx * vp.vx + v1.vy * vp.vy) >= 0)
			{
				//collision
				//amount to move back moving ball
				moveBack = MathF.Sqrt(Config.BALL_RAD_PX_SQUARED - vn.len * vn.len);
				
				//vector from ball1 starting point to its coordinates when collision happens
				v4.p0 = v1.p0;
				v4.p1 = new Point(vn.p0.x - moveBack * v1.dx, vn.p0.y - moveBack * v1.dy);
				v4.updateComponentsFromPoints();
				v4.makeVector();
				
				/*var v111:MyVector = new MyVector();
				v111.p0 = v4.p1.clone();
				v111.p1 = new Point(a.x, a.y);
				v111.updateComponentsFromPoints();
				v111.makeVector();
				//trace ("getTimeToNextBallAngleCollision: v111 len:" + v111.len)
				trace ("getTimeToNextBallAngleCollision: p0:" + v111.p0)
				trace ("getTimeToNextBallAngleCollision: p1:" + v111.p1)*/
				return (v4.len / v1.len) * Config.SPEED_UPDATE_DELTA;
			}
			return float.MaxValue;
		}


		
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////---------------------------------ШАРЫ с ЛУЗАМИ---------------------------------///////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		/**
		 * Проверить, есть ли столкновение шара и луз. Если есть, то обновить timeToNextColl и collisionObj
		 * @param	b шар
		 */
		private void checkBallWithPocketsCollisionTime(Ball b)
		{
			for (var i = 0; i < pockets.Count; i++) 
			{
				var timeToNextColl = getTimeToNextBallPocketCollision(b, pockets[i]);
			
				//проверяем, если время до столкновения меньше текущего, то обновляем.
				if (timeToNextColl < float.MaxValue && timeToNextColl <= collisions.TimeToCollision) 
				{
					collisions.AddBallWithPocketCollision(b, pockets[i], timeToNextColl);
				}
			}
		}


		/**
		 * Возвращает время в миллисекундах, через которое шар столкнотся с лузой. Если не столкнутся, то возвращает  Number.MAX_VALUE
		 * @param	b
		 * @param	p
		 * @return
		 */
		private float getTimeToNextBallPocketCollision(Ball b, Pocket p)
		{
			v1 = b.v; 	//TODO можно будет сразу на входе в параметрах принимать вектора, а не шары.
			
			//vector between center points of ball
			vc.p0 = v1.p0;
			vc.p1.x = p.x;
			vc.p1.y = p.y;
			vc.updateComponentsFromPoints();
			vc.makeVector();
			
			//sum of radius with balls distance
		
			var pen = Config.POCKET_RAD_PX - vc.len;
			
			//check if balls collide at start
			if (pen >= 0.001)
			{
				ToLog(/*GameTime.getInstance().getServerTime() + " " + int(currDt) + */" ERROR getTimeToNextBallPocketCollision: collide at start. b:" + b.Number + " p:" + p.x + "," + p.y + " pen:" + pen); 
			}
			
			//projection of vc on v1
			vp = MyVector.projectVector(vc, v1.dx, v1.dy);
			
			//vector to center of pocket in direction of movement vectors normal
			vn.p0 = new Point(v1.p0.x + vp.vx, v1.p0.y + vp.vy);
			vn.p1.x = p.x;
			vn.p1.y = p.y;
			vn.updateComponentsFromPoints();
			vn.makeVector();
			
			//check if vn is shorter then combined radiuses
			diff = ( Config.POCKET_RAD_PX ) - vn.len;
			
			if (diff >= 0 && (v1.vx * vp.vx + v1.vy * vp.vy) >= 0)
			{
				//collision
				//amount to move back moving ball
				moveBack = MathF.Sqrt(Config.POCKET_RAD_PX_SQUARED - vn.len * vn.len);
				
				//vector from ball1 starting point to its coordinates when collision happens
				v4.p0 = v1.p0;
				v4.p1 = new Point(vn.p0.x - moveBack * v1.dx, vn.p0.y - moveBack * v1.dy);
				v4.updateComponentsFromPoints();
				v4.makeVector();
				
				/*var v111:MyVector = new MyVector();
				v111.p0 = v4.p1.clone();
				v111.p1 = new Point(a.x, a.y);
				v111.updateComponentsFromPoints();
				v111.makeVector();
				//trace ("getTimeToNextBallAngleCollision: v111 len:" + v111.len)
				trace ("getTimeToNextBallAngleCollision: p0:" + v111.p0)
				trace ("getTimeToNextBallAngleCollision: p1:" + v111.p1)*/
				return (v4.len / v1.len) * Config.SPEED_UPDATE_DELTA;
			}	
			return float.MaxValue;
		}


		
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		
		
		
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////--------------------------------ПРИМЕНИТЬ КОЛЛИЗИИ К ШАРАМ--------------------------------///////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
				
		private void ProcessCollisions(ref ShotTickResult shotTickResult)
		{
			if (collisions.BallsCollisions.Count == 0 && collisions.BallWallCollisions.Count == 0 && collisions.BallAngleCollisions.Count == 0 && collisions.BallPocketCollisions.Count == 0)
			{
				ToLog("calcCollisionObject ERROR: collisionCount == 0");
			}

			foreach (var pair in collisions.BallsCollisions)
			{
				applyCollisionToBalls(pair[0], pair[1], ref shotTickResult);
            }

			foreach (var pair in collisions.BallWallCollisions)
			{
                applyBallWallCollision(pair.Key, pair.Value, ref shotTickResult);
            }

			foreach (var pair in collisions.BallAngleCollisions)
			{
                applyBallAngleCollision(pair.Key, pair.Value, ref shotTickResult);
            }

			foreach (var pair in collisions.BallPocketCollisions)
			{
                applyBallPocketCollision(pair.Key, pair.Value, ref shotTickResult);
            }
		}
		
		/**
		 * Посчитать столкновение шаров и обновить векторы после столкновения.
		 * @param	шар1
		 * @param	шар2
		 * @return
		 */
		private void applyCollisionToBalls(Ball b1, Ball b2, ref ShotTickResult shotTickResult)
		{
			shotCalculateResult.BallsCollisionCount++;
			shotCalculateResult.FirstCollisionBallNum ??= (b1.Number == cueBallNum) ? b2.Number : b1.Number;
			
			//vector between center points of ball
			vc.p0 = b1.v.p0;
			vc.p1 = b2.v.p0;
			vc.updateComponentsFromPoints();
			vc.makeVector();
			
			//sum of radius with balls distance
		
			var pen = Config.BALL_DIAM_PX - vc.len;
			
			//check if balls are not collide at start
			if (pen < -0.01)
			{
				ToLog("ERROR applyCollisionToBalls: balls are not collide. vc.len:" + vc.len);
			}
			else
			{
				newv2Balls = MyVector.bounceBalls(b1.v, b2.v, vc);
                b1.v.vx = newv2Balls.vx1 * Config.BALL_COLLISION_POWER_MISS;
				b1.v.vy = newv2Balls.vy1 * Config.BALL_COLLISION_POWER_MISS;
				b2.v.vx = newv2Balls.vx2 * Config.BALL_COLLISION_POWER_MISS;
				b2.v.vy = newv2Balls.vy2 * Config.BALL_COLLISION_POWER_MISS;
				b1.v.updatePointsFromComponents();
				b2.v.updatePointsFromComponents();
				b1.v.makeVector();
				b2.v.makeVector();

                shotTickResult.TryChangeMaxBallsCollPower(newv2Balls.power);

                ToLog(/*GameTime.getInstance().getServerTime() + " " + int(currDt) + */"BB Collision Applied:" + b1.Number + " " + b2.Number);
			}
			
			b1.NeedUpdateState = true;
			b2.NeedUpdateState = true;
		}
		
		
		
		private float cos;
		/**
		 * Посчитать столкновение и обновить векторы после столкновения. b1 - активный шар (движется), b2 - шар в состоянии покоя
		 * @param	шар
		 * @param	стена
		 * @return
		 */
		private void applyBallWallCollision(Ball b, MyVector w, ref ShotTickResult shotTickResult)
		{
			if (shotCalculateResult.BallsCollisionCount > 0) shotCalculateResult.WallsCollisionAfterBallsCollisionCount++;
			shotCalculateResult.WallsCollisionCount++;

            BallWallIntersectResult ir = getBallWallIntersectionInfo(b.v.p0.x, b.v.p0.y, b.v.p1.x, b.v.p1.y, w.p0.x, w.p0.y, w.p1.x, w.p1.y);
			
			//check if ball vs wall are not collide
			if (ir.IsNeedProcessCollision == false)
			{
				ToLog("ERROR calcCollision: ball vs wall are not collide. BallNum:" + b.Number + " WallP0:" + w.p0.x + ";" + w.p0.y + " U1:" + ir.u1 + " U2:" + ir.u2);
                BallWallIntersectResult ir2 = getBallWallIntersectionInfo(b.v.p0.x, b.v.p0.y, b.v.p1.x, b.v.p1.y, w.p0.x, w.p0.y, w.p1.x, w.p1.y);
            }
			else
			{
				//trace ("BW Collision before Applied:" + " bP0X:" + b.v.p0.x + " bP0Y:" + b.v.p0.y + " bP1X" + b.v.p1.x + " bP1Y" + b.v.p1.y);
				
				//косинус угла между вектором поступательного движения и стеной (чем перпендикулярнее, тем ближе к 0)
				cos = MathF.Abs(MyVector.getDotP(w, b.v) / ( w.len * b.v.len));
				//trace ("cos = " + String(MyVector.getDotP(w, b.v) / ( w.len * b.v.len)));
				newv1Ball = MyVector.bounceBallFromWall(b.v, w);
				b.v.vx = newv1Ball.vx * (Config.WALL_ELASTIC + (1 - Config.WALL_ELASTIC) * cos);
				b.v.vy = newv1Ball.vy * (Config.WALL_ELASTIC + (1 - Config.WALL_ELASTIC) * cos);
				b.v.p0.x = ir.x;	//чтобы избавиться от погрешностей
				b.v.p0.y = ir.y;
				
				if (b.SideSpin != 0)
				{
					b.v.vx += w.dx * b.SideSpin * (1 - cos);
					b.v.vy += w.dy * b.SideSpin * (1 - cos);
				}
				b.SideSpin *= cos;
				
				b.v.updatePointsFromComponents();
				b.v.makeVector();
				
				//Если имеется вертикальное вращение, то правим его в зависимости от угла между вектором вращения и стеной 
				//(чем ближе к 90 градусам, тем более сильно гасится вращение)
				if (b.vVertSpin.len > 1)
				{
					//косинус угла между вектором вертикального вращения и стеной
					cos = MathF.Abs(MyVector.getDotP(w, b.vVertSpin) / ( w.len * b.vVertSpin.len));
					//trace ("cos = " + cos);
					b.vVertSpin.vx *= (0.2f + 0.8f * cos) * Config.VERTICAL_ROTATION_WALL_ABSORB;
					b.vVertSpin.vy *= (0.2f + 0.8f * cos) * Config.VERTICAL_ROTATION_WALL_ABSORB;
					b.vVertSpin.makeVector();
				}

				shotTickResult.TryChangeMaxWallsCollPower(newv1Ball.power);
				
				//trace ("BW Collision after Applied:" + " bP0X:" + b.v.p0.x + " bP0Y:" + b.v.p0.y + " bP1X" + b.v.p1.x + " bP1Y" + b.v.p1.y);
				ToLog(/*GameTime.getInstance().getServerTime() + " " + int(currDt) + */"BW Collision Applied. BallNum:" + b.Number + " wallP0:" + w.p0.x + ";" + w.p0.y);
			}
			
			b.NeedUpdateState = true;
		}
		
		private MyVector vOrt = new MyVector();
		/**
		 * Посчитать столкновение шара и угла. b - активный шар (движется), a - угол
		 * @param	шар
		 * @param	шар-угол
		 * @return
		 */
		private void applyBallAngleCollision(Ball b, Angle a, ref ShotTickResult shotTickResult)
		{
			if (shotCalculateResult.BallsCollisionCount > 0) shotCalculateResult.WallsCollisionAfterBallsCollisionCount++;
			shotCalculateResult.WallsCollisionCount++;
			
			//vector between center points of ball
			vc.p0 = b.v.p0;
			vc.p1.x = a.x;
			vc.p1.y = a.y;
			vc.updateComponentsFromPoints();
			vc.makeVector();
			
			//sum of radius with balls distance
		
			var pen = Config.BALL_RAD_PX - vc.len;
			
			//check if balls are not collide at start
			ToLog("BALL_ANGLE:" + pen);
			if (pen < -0.01)
			{
				ToLog("ERROR applyBallAngleCollision: ball with angle are not collide. vc.len:" + vc.len);
			}
			else
			{
				//trace ("BA Collision before Applied:" + " bP0X:" + b.v.p0.x + " bP0Y:" + b.v.p0.y + " bP1X" + b.v.p1.x + " bP1Y" + b.v.p1.y);
				
				vOrt.vx = vc.rx;
				vOrt.vy = vc.ry;
				vOrt.updatePointsFromComponents();
				vOrt.makeVector();
				//косинус угла между вектором поступательного движения и касательной к углу
				cos = MathF.Abs(MyVector.getDotP(vOrt, b.v) / ( vOrt.len * b.v.len));
				//trace ("ASDASD" + String(Config.WALL_ELASTIC + (1 - Config.WALL_ELASTIC) * cos));
				newv1Ball = MyVector.bounceBallFromAngle(b.v, vc);
				
				b.v.vx = newv1Ball.vx * (Config.WALL_ELASTIC + (1 - Config.WALL_ELASTIC) * cos);
				b.v.vy = newv1Ball.vy * (Config.WALL_ELASTIC + (1 - Config.WALL_ELASTIC) * cos);
				//b.v.p0.x = ir.x;	//чтобы избавиться от погрешностей
				//b.v.p0.y = ir.y;
				b.v.updatePointsFromComponents();
				b.v.makeVector();
				
				//Если имеется вертикальное вращение, то правим его в зависимости от угла между вектором вращения и касательной в точку касания
				//(чем ближе к 90 градусам, тем более сильно гасится вращение)
				if (b.vVertSpin.len > 1)
				{
					//косинус угла между вектором движения и касательной к углу
					cos = MathF.Abs(MyVector.getDotP(vOrt, b.vVertSpin) / ( vOrt.len * b.vVertSpin.len));
					ToLog("cosVA = "  + cos);
					b.vVertSpin.vx *= (0.2f + 0.8f * cos) * Config.VERTICAL_ROTATION_WALL_ABSORB;
					b.vVertSpin.vy *= (0.2f + 0.8f * cos) * Config.VERTICAL_ROTATION_WALL_ABSORB;
					b.vVertSpin.makeVector();
				}

				shotTickResult.TryChangeMaxWallsCollPower(newv1Ball.power);
				
				//trace ("BA Collision after Applied:" + " bP0X:" + b.v.p0.x + " bP0Y:" + b.v.p0.y + " bP1X" + b.v.p1.x + " bP1Y" + b.v.p1.y);
				ToLog(/*GameTime.getInstance().getServerTime() + " " + int(currDt) + */"BA Collision Applied: b:" + b.Number + " a:" + a.x + "," + a.y);
			}
			
			b.NeedUpdateState = true;
		}
		
		
		/**
		 * Посчитать столкновение шара и лузы. b - активный шар (движется), p - луза
		 * @param	шар
		 * @param	шар-луза
		 * @return
		 */
		private void applyBallPocketCollision(Ball b, Pocket p, ref ShotTickResult shotTickResult)
		{
			//vector between center points of ball
			vc.p0 = b.v.p0;
			vc.p1.x = p.x;
			vc.p1.y = p.y;
			vc.updateComponentsFromPoints();
			vc.makeVector();
			
			//sum of radius with balls distance
		
			var pen = Config.POCKET_RAD_PX - vc.len;
			
			//check if balls are not collide at start
			ToLog("BALL_POCKET:" + pen);
			if (pen < -0.01)
			{
				ToLog("ERROR applyBallPocketCollision: ball with pocket are not collide. vc.len:" + vc.len);
			}
			else
			{
				shotCalculateResult.PocketedBalls.Add(b.Number);
				shotCalculateResult.PocketedBallsPockets.Add(p.pocketNum);
				
				//TODO Добавить анимацию удаления шара
				b.IsRemoved = true;
				b.PocketRemoveTo = p;
				b.RemoveDeltaTime = currDt;

				shotTickResult.TryChangeMaxPocketedPower(b.v.len / Config.MAX_SHOT_POWER);
				shotTickResult.AddPocketedBall(b);
			}
			
			b.NeedUpdateState = true;
			
			//toLog("Collision applyed. Ball:" + b.bNumber + ";pocket:" + p.x + ";" + p.y);
		}
		
		
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		
		
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////-------------------------------АКТИВНЫЕ/СПЯЩИЕ/ЗАБИТЫЕ ШАРЫ--------------------------------///////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		
		
		
		private List<Ball> activeBalls = new List<Ball>();
		
		private void UpdateAllBallsState()
		{
			foreach (var b in Balls) 
			{
				if (b.NeedUpdateState == false) continue;
				if (b.IsRemoved)
				{
					removeBallFromTable(b);
				}
				else if (b.IsSleep)
				{
					if (b.v.len > 0) addActiveBall(b);
				}
				else 
				{
					if (b.v.len == 0) removeActiveBall(b);
				}
				
				b.NeedUpdateState = false;
			}
		}
		
		private void addActiveBall(Ball b)
		{
			if (activeBalls.IndexOf(b) >= 0) 
			{ 
				ToLog("addActiveBall ERROR!");
				return;
			}
			activeBalls.Add(b);
			b.IsSleep = false;
		}
		
		private void removeActiveBall(Ball b)
		{
			if (activeBalls.Remove(b) == false)
			{
				ToLog("removeActiveBall ERROR!");
				return;
			}
			b.IsSleep = true;
		}
		
		private void removeBallFromTable(Ball b)
		{
			if (activeBalls.Remove(b))
			{
				b.IsSleep = true;
			}
		}
		
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////----------------------------------ОБНОВЛЕНИЕ СКОРОСТЕЙ------------------------------------///////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		/**
		 * обнулить время удара
		 */
		private void resetShotParams()
		{
			currShotDuration = 0;
			timeToVelUpdate = 0;
			updateNextTimeToVelUpdate();
			lastDt = 0;
			
			ToLog("SHOT RESETED.");
		}
		/**
		 * 
		 */
		private void updateNextTimeToVelUpdate()
		{
			timeToVelUpdate += Config.SPEED_UPDATE_DELTA;
		}
		
		private void UpdateBallsVelocities()
		{
			for (var i = 0; i < activeBalls.Count; i++) 
			{
				(activeBalls[i] as Ball).UpdateVelocities();
			}
			updateNextTimeToVelUpdate();
			
			//trace (currShotDuration + ": Velocities updated.");
		}
		
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
		
		


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////----------------------------------ПРИЦЕЛЬНАЯ ЛИНИЯ------------------------------------///////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		

		private float aimTimeToColl = float.MaxValue;
		private Ball aimCollBall = null;
		private MyVector aimCollWall = null;
		private Angle aimCollAngle = null;
		private Pocket aimCollPocket = null;
		private AimObject aimObjectResult = new AimObject();
		private Ball bAimTmp = new Ball(0);
		/**
		 * Возвращает параметры для отображения прицельной линии
		 * @param 	Копия шара битка (номер такой же), с новым направлением движения (направление прицела)
		 * @return Объект для отображения прицельной линии
		 */
		public AimObject GetAimObject(Ball b)
		{
			aimObjectResult.AimBallX0 = b.v.p0.x;
			aimObjectResult.AimBallY0 = b.v.p0.y;
			aimTimeToColl = float.MaxValue;
			/*trace ("-------getAimObj--------");
			trace ("bp0x:" + b.v.p0.x + " bp0y:" + b.v.p0.y );
			trace ("bp1x:" + b.v.p1.x + " bp1y:" + b.v.p1.y );*/
			
			float aimTimeToCollTmp;
			
			for (var i = 0; i < Balls.Count; i++) 
			{
				if (b.Number != (Balls[i] as Ball).Number && (Balls[i] as Ball).IsRemoved == false)
				{
					aimTimeToCollTmp = getTimeToNextBallsCollision(b, Balls[i]);
					if (aimTimeToColl > aimTimeToCollTmp)
					{
						aimTimeToColl = aimTimeToCollTmp;
						aimCollBall = Balls[i];
						aimCollWall = null;
						aimCollAngle = null;
						aimCollPocket = null;
					}
				}
			}
			
			//trace ("aimTimeToCol1l: " + aimTimeToColl);
			var vectorProduct = 0f;
			for (var i = 0; i < walls.Count; i++)
			{
				aimTimeToCollTmp = getTimeToBallWithWallCollision(b, walls[i], true);
				if (aimTimeToColl > aimTimeToCollTmp)
				{
					if (aimTimeToCollTmp == 0)
					{
						//если P2 шара слева от вектора стены (векторное произведение < 0), значит надо обрабатывать коллизию, иначе коллизия уже обработана и шар уже движется в направлении от стены
						vectorProduct = ((b.v.p1.x - b.v.p0.x) * (walls[i].p1.y - walls[i].p0.y) - (b.v.p1.y - b.v.p0.y) * (walls[i].p1.x - walls[i].p0.x));
					}
					if (aimTimeToCollTmp != 0 || vectorProduct > 0 )
					{
						aimTimeToColl = aimTimeToCollTmp;
						aimCollWall = walls[i];
						aimCollBall = null;
						aimCollAngle = null;
						aimCollPocket = null;
					}
				}
				vectorProduct = 0;
			}
			//trace ("aimTimeToCol1l: " + aimTimeToColl);
			
			for (var i = 0; i < angles.Count; i++) 
			{
				aimTimeToCollTmp = getTimeToNextBallAngleCollision(b, angles[i]);
				if (aimTimeToColl > aimTimeToCollTmp)
				{
					aimTimeToColl = aimTimeToCollTmp;
					aimCollWall = null;
					aimCollBall = null;
					aimCollAngle = angles[i];
					aimCollPocket = null;
				}
			}
			//trace ("aimTimeToColl3: " + aimTimeToColl);
			
			for (var i = 0; i < pockets.Count; i++) 
			{
				aimTimeToCollTmp = getTimeToNextBallPocketCollision(b, pockets[i]);
				if (aimTimeToColl > aimTimeToCollTmp)
				{
					aimTimeToColl = aimTimeToCollTmp;
					aimCollWall = null;
					aimCollBall = null;
					aimCollAngle = null;
					aimCollPocket = pockets[i];
				}
			}
			//trace ("aimTimeToColl4: " + aimTimeToColl);
			
			aimObjectResult.AimBallX = b.v.p0.x + b.v.vx * aimTimeToColl / Config.SPEED_UPDATE_DELTA;
			aimObjectResult.AimBallY = b.v.p0.y + b.v.vy * aimTimeToColl / Config.SPEED_UPDATE_DELTA;
			aimObjectResult.FirstCollBallNum = (aimCollBall != null) ? aimCollBall.Number : -1;
			
			if (aimCollBall != null)
			{
				bAimTmp.v.p0.x = aimObjectResult.AimBallX;
				bAimTmp.v.p0.y = aimObjectResult.AimBallY;
				bAimTmp.v.vx = b.v.dx;
				bAimTmp.v.vy = b.v.dy;
				bAimTmp.v.updatePointsFromComponents();
				bAimTmp.v.makeVector();
				vc.p0 = bAimTmp.v.p0;
				vc.p1 = aimCollBall.v.p0;
				vc.updateComponentsFromPoints();
				vc.makeVector();
				
				newv2Balls = MyVector.bounceBalls(bAimTmp.v, aimCollBall.v, vc);
				//trace (newv2Balls.vx1, newv2Balls.vy1);
				//trace ("   " + newv2Balls.vx2 + " " + newv2Balls.vy2);
				
				aimObjectResult.AimBallBounceVx = newv2Balls.vx1;
				aimObjectResult.AimBallBounceVy = newv2Balls.vy1;
				aimObjectResult.CollBallBounceVx = newv2Balls.vx2;
				aimObjectResult.CollBallBounceVy = newv2Balls.vy2;
				aimObjectResult.CollBallX0 = aimCollBall.v.p0.x;
				aimObjectResult.CollBallY0 = aimCollBall.v.p0.y;
				//b1.v.vx = newv2Balls.vx1;
				//b1.v.vy = newv2Balls.vy1;
				//b2.v.vx = newv2Balls.vx2;
				//b2.v.vy = newv2Balls.vy2;
				//b1.v.updatePointsFromComponents();
				//b2.v.updatePointsFromComponents();
				//b1.v.makeVector();
				//b2.v.makeVector();
			}
			
			//trace ("AIMRESULT: " + aimObjectResult.aimBallX + " " + aimObjectResult.aimBallY);
			return aimObjectResult;
		}
		
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////	

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////-----------------------------------ПОЗИЦИИ ШАРОВ-------------------------------------------///////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		/**
		 * Координата Х шара
		 * @param	bNum
		 * @return
		 */
		public float getBallX(int bNum = 0)
		{
			if (Balls[bNum] == null) { ToLog("ERROR:GameCore.getBallX() balls[bNum] == null, bnum:" + bNum); return 0; }
			return Balls[bNum].v.p0.x;
		}
		/**
		 * Координата Y шара
		 * @param	bNum
		 * @return
		 */
		public float getBallY(int bNum = 0)
		{
			if (Balls[bNum] == null) { ToLog("ERROR:GameCore.getBallY() balls[bNum] == null, bnum:" + bNum); return 0; }
			return Balls[bNum].v.p0.y;
		}
		
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////	

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////-----------------------ИНИЦИАЛИЗАЦИЯ СТЕН И ШАРОВ СТЕНЫ-----------------------------------///////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		private void createWalls() 
		{
			foreach (var coord in wallsCoord)
			{
				var v = new MyVector();
				v.setP0(coord[0].x, coord[0].y);
				v.setP1(coord[1].x, coord[1].y);
				v.updateComponentsFromPoints();
				v.makeVector();
				RealWalls.Add(v);
			}
			
			calcWallsAndAngles();
		}
		
		/**
		 * Просчитать новые стены, подвинутые на радиус шара, а также создать шары-углы
		 */
		private void calcWallsAndAngles()
        {
            walls.Clear();

            for (var i = 0; i < RealWalls.Count; i++) 
			{
				walls.Add(getDisplacedWall(RealWalls[i]));
				
				if (i > 0)
				{
					cutIntersections(walls[i-1], walls[i]);
				}
				if (i == RealWalls.Count - 1)
				{
					cutIntersections(walls[i], walls[0]);
				}
			}
			
			addWallAngles();
		}
		
		//Получить вектор стены, смещенной на радиус шара вправо относительно направления
		private MyVector getDisplacedWall(MyVector realWall)
		{
			//trace ("getDisplacedWall")
			//trace ("w1: p0.x:" + realWall.p0.x + " p0.y:" + realWall.p0.y + " p1.x:" + realWall.p1.x + " p1.y:" + realWall.p1.y);
			var res = new MyVector();
			
			res.p0.x 	= realWall.p0.x + realWall.rx * Config.BALL_RAD_PX;
			res.p0.y 	= realWall.p0.y + realWall.ry * Config.BALL_RAD_PX;
			res.vx 		= realWall.vx;
			res.vy		= realWall.vy;
			
			res.updatePointsFromComponents();
			res.makeVector();
			
			//trace ("res: p0.x:" + res.p0.x + " p0.y:" + res.p0.y + " p1.x:" + res.p1.x + " p1.y:" + res.p1.y);
			
			return res;
		}
		
		//Обрезать излишки новых стен после смещения
		private void cutIntersections(MyVector v1, MyVector v2)
		{
			//trace ("w1: p0.x:" + v1.p0.x + " p0.y:" + v1.p0.y + " p1.x:" + v1.p1.x + " p1.y:" + v1.p1.y);
			//trace ("w2: p0.x:" + v2.p0.x + " p0.y:" + v2.p0.y + " p1.x:" + v2.p1.x + " p1.y:" + v2.p1.y);
			
			var denumenator = (v2.p1.y - v2.p0.y) * ( v1.p1.x - v1.p0.x) - (v2.p1.x - v2.p0.x) * (v1.p1.y - v1.p0.y);
			
			//если прямые параллельны:
			if (denumenator == 0)
			{
				return;
			}
			else
			{
				var u1 = (((v2.p1.x - v2.p0.x) * (v1.p0.y - v2.p0.y)) - ((v2.p1.y - v2.p0.y) * (v1.p0.x - v2.p0.x))) / denumenator;
				var u2 = (((v1.p1.x - v1.p0.x) * (v1.p0.y - v2.p0.y)) - ((v1.p1.y - v1.p0.y) * (v1.p0.x - v2.p0.x))) / denumenator;
				
				if (u1 <= 0 || u1 == 1 || u2 >= 1)
				{
					ToLog("Walls cutIntersections ERROR: u1:" + u1 + " u2:" + u2); 
					return;
				}
				else
				{
					var intX = v1.p0.x + u1 * (v1.p1.x - v1.p0.x);
					var intY = v1.p0.y + u1 * (v1.p1.y - v1.p0.y);
					if (u1 > 0 && u1 < 1)
					{
						v1.p1.x = intX;
						v1.p1.y = intY;
						v1.updateComponentsFromPoints();
						v1.makeVector();
					}
					if (u2 > 0 && u2 < 1)
					{
						v2.p0.x = intX;
						v2.p0.y = intY;
						v2.updateComponentsFromPoints();
						v2.makeVector();
					}
				}
			}
			
			//trace (v1.p0.x + "," + v1.p0.y);
		}
		
		//Добавить шары-углы относительно стен
		private void addWallAngles()
		{
			for (var i = 0; i < RealWalls.Count; i++) 
			{
				if (i > 0)
				{
					checkTwoWallsToAddAngle(RealWalls[i-1], RealWalls[i]);
				}
				if (i == RealWalls.Count - 1)
				{
					checkTwoWallsToAddAngle(RealWalls[i], RealWalls[0]);
				}
			}
		}
		//проверить, нужен ли щар-угол в точке пересечения двух стен (если p1 второго вектора расположена левее первого вектора, то нужен)
		private void checkTwoWallsToAddAngle(MyVector v1, MyVector v2)
		{
			var vectorProduct = ((v1.p1.x - v1.p0.x) * (v2.p1.y - v2.p0.y) - (v1.p1.y - v1.p0.y) * (v2.p1.x - v2.p0.x));
			//trace ("vectorProduct:" + vectorProduct);
			
			if (vectorProduct < 0) 
				angles.Add(new Angle(v1.p1.x, v1.p1.y, Config.BALL_RAD_PX));
		}
		
		//Создать лузы
		private void createPockets()
        {
            pockets.Clear();

			for (int i = 0; i < pocketsCoords.Count; i++)
			{
				var pocketCoord = pocketsCoords[i][0];
				var pocketRemoveBallVEctor = pocketsCoords[i][1];
				pockets.Add(new Pocket(0, pocketCoord.x, pocketCoord.y, Config.POCKET_RAD_PX, pocketRemoveBallVEctor.x, pocketRemoveBallVEctor.y));
			}
		}
		
		/**
		 * Возвращает координаты лузы
		 * @param	pocketNum
		 * @return
		 */
		public Point getPocketCoord(int pocketNum)
		{
			return new Point(pockets[pocketNum].x, pockets[pocketNum].y);
		}
		
		
		
		
		/**
		 * Установить шары в начальные позиции
		 */
		public void PrepeareNewGame(int posNum) 
		{
			MatchShotsCount = 0;

			Balls.Clear();
			for (var i = 0; i <= 15 ; i++) 
			{
				Balls.Add(new Ball(i));
			}

			(Balls[0] as Ball).SetPosition(Config.cueBallPosX, Config.cueBallPosY);
			var x0 = 158f + (posNum - 5);
			var y0 = 0f + (posNum - 5);

			(Balls[9] as Ball).SetPosition(x0, y0);
			var step = MathF.Sqrt(Config.BALL_DIAM_PX_SQUARED - Config.BALL_RAD_PX_SQUARED) + 0.1f + (0.5f * posNum / 10f);
			(Balls[12] as Ball).SetPosition(x0 + step, y0 - Config.BALL_RAD_PX);
			(Balls[7] as Ball).SetPosition(x0 + step, y0 + Config.BALL_RAD_PX);
			(Balls[1] as Ball).SetPosition(x0 + 2 * step, y0 - 2 * Config.BALL_RAD_PX);
			(Balls[8] as Ball).SetPosition(x0 + 2 * step, y0);
			(Balls[15] as Ball).SetPosition(x0 + 2 * step, y0 + 2 * Config.BALL_RAD_PX);
			(Balls[14] as Ball).SetPosition(x0 + 3 * step, y0 - 3 * Config.BALL_RAD_PX);
			(Balls[3] as Ball).SetPosition(x0 + 3 * step, y0 - 1 * Config.BALL_RAD_PX);
			(Balls[10] as Ball).SetPosition(x0 + 3 * step, y0 + 1 * Config.BALL_RAD_PX);
			(Balls[6] as Ball).SetPosition(x0 + 3 * step, y0 + 3 * Config.BALL_RAD_PX);
			(Balls[5] as Ball).SetPosition(x0 + 4 * step, y0 - 4 * Config.BALL_RAD_PX);
			(Balls[4] as Ball).SetPosition(x0 + 4 * step, y0 - 2 * Config.BALL_RAD_PX);
			(Balls[13] as Ball).SetPosition(x0 + 4 * step, y0);
			(Balls[2] as Ball).SetPosition(x0 + 4 * step, y0 + 2 * Config.BALL_RAD_PX);
			(Balls[11] as Ball).SetPosition(x0 + 4 * step, y0 + 4 * Config.BALL_RAD_PX);
		}
		
		
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
		

		private void roundBallsCoord()
		{
			for (var i = 0; i < Balls.Count; i++) 
			{
				(Balls[i] as Ball).v.p0.x = (Balls[i] as Ball).v.p1.x = (int)((Balls[i] as Ball).v.p0.x * Config.COORD_ROUND_TO) / (float) Config.COORD_ROUND_TO;
				(Balls[i] as Ball).v.p0.y = (Balls[i] as Ball).v.p1.y = (int)((Balls[i] as Ball).v.p0.y * Config.COORD_ROUND_TO) / (float) Config.COORD_ROUND_TO;
			}
		}
		
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/**
		 * Установить игровую позицию шаров на столе
		 * @param	posParams
		 */
		public void SetBallsPosition(List<BallData> ballPositions)
		{
			if (ballPositions.Count != 16) 
			{
				ToLog("setBallsPosition Error. a.length:" + ballPositions.Count);
				return;
			}
			
			resetShotParams();
			activeBalls.Clear();

			foreach (var ballData in ballPositions)
			{
                var b = Balls[ballData.Number];
				b.ResetParams();
				b.IsRemoved = ballData.IsRemoved;
                b.SetPosition(ballData.X, ballData.Y);
				b.v.vx = 0;
				b.v.vy = 0;
				b.v.updatePointsFromComponents();

            }
		}

        /////////////////////ДЛЯ ТЕСТОВ:////////////////////
        private void ToLog(string s)
		{
            //Debug.Log("GameCore: " + s);
        }
        private void ToLogError(string s)
		{
            //Debug.LogError("GameCore: " + s);
        }

        ////////////////////КООРДИНАТЫ////////////////////

        private List<Point[]> wallsCoord = new List<Point[]>()
		{
			{new Point[]{new Point(-285f, -155f), new Point(-23f, -155f)}},
			{new Point[]{new Point(-23f, -155f), new Point(-13f, -195f)}},
			{new Point[]{new Point(-13f, -195f), new Point(13f, -195f)}},
			{new Point[]{new Point(13f, -195f), new Point(23f, -155f)}},
			{new Point[]{new Point(23f, -155f), new Point(285f, -155f)}},
			{new Point[]{new Point(285f, -155f), new Point(310f, -180f)}},
			{new Point[]{new Point(310f, -180f), new Point(335f, -155f)}},
			{new Point[]{new Point(335f, -155f), new Point(310f, -130f)}},
			{new Point[]{new Point(310f, -130f), new Point(310f, 130f)}},
			{new Point[]{new Point(310f, 130f), new Point(335f, 155f)}},
			{new Point[]{new Point(335f, 155f), new Point(310f, 180f)}},
			{new Point[]{new Point(310f, 180f), new Point(285f, 155f)}},
			{new Point[]{new Point(285f, 155f), new Point(23f, 155f)}},
			{new Point[]{new Point(23f, 155f), new Point(13f, 195f)}},
			{new Point[]{new Point(13f, 195f), new Point(-13f, 195f)}},
			{new Point[]{new Point(-13f, 195f), new Point(-23f, 155f)}},
			{new Point[]{new Point(-23f, 155f), new Point(-285f, 155f)}},
			{new Point[]{new Point(-285f, 155f), new Point(-310f, 180f)}},
			{new Point[]{new Point(-310f, 180f), new Point(-335f, 155f)}},
			{new Point[]{new Point(-335f, 155f), new Point(-310f, 130f)}},
			{new Point[]{new Point(-310f, 130f), new Point(-310f, -130f)}},
			{new Point[]{new Point(-310f, -130f), new Point(-335f, -155f)}},
			{new Point[]{new Point(-335f, -155f), new Point(-310f, -180f)}},
			{new Point[]{new Point(-310f, -180f), new Point(-285f, -155f)}},

        };
		private List<Point[]> pocketsCoords = new List<Point[]>()
		{
            {new Point[]{new Point(-317.5f,-162.5f), new Point(1f,1f)}},
			{new Point[]{new Point(0f,-175f), new Point(0f,1f)}},
			{new Point[]{new Point(317.5f,-162.5f), new Point(-1f,1f)}},
			{new Point[]{new Point(317.5f,162.5f), new Point(-1f,-1f)}},
			{new Point[]{new Point(0f,175f), new Point(0f,-1f)}},
			{new Point[]{new Point(-317.5f,162.5f), new Point(1f,-1f)}},
        };

    }
}
