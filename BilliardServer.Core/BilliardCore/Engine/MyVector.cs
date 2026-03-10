using System;

namespace Kborod.BilliardCore
{
    public class MyVector
    {
        //начальная точка вектора
        public Point p0;

        //конечная точка вектора
        public Point p1;

        //компонента x вектора
        public Fixed64 vx;

        //компонента y вектора
        public Fixed64 vy;

        //компонента x единичного вектора
        public Fixed64 dx;

        //компонента y единичного вектора
        public Fixed64 dy;


        //x правая нормаль 
        public Fixed64 rx;
        //y правая нормаль
        public Fixed64 ry;

        //x левая нормаль 
        public Fixed64 lx = Fixed64.Zero;
		//y левая нормаль 
		public Fixed64 ly = Fixed64.Zero;

        //длина вектора
        public Fixed64 len;
		

        public void setP0(Fixed64 p0X, Fixed64 p0Y)
		{
			p0.x = p0X;
			p0.y = p0Y;
		}

		public void setP1(Fixed64 p1X, Fixed64 p1Y)
		{
			p1.x = p1X;
			p1.y = p1Y;
		}

		public void updatePointsFromComponents()
		{
			p1.x = p0.x + vx;
			p1.y = p0.y + vy;
			//holdVector();
		}
		
		public void updateComponentsFromPoints()
		{
			vx = p1.x - p0.x;
			vy = p1.y - p0.y;
			//holdVector();
		}
		
		public void makeVector()
		{
			//length of vector
			updateLen();
			//normalized unit-sized components
			if (len > Fixed64.Zero)
			{
				dx = vx / len;
				dy = vy / len;
			}
			else
			{
				dx = Fixed64.Zero;
				dy = Fixed64.Zero;
			}
			//right hand normal
			rx = -dy;
			ry = dx;
			//left hand normal
			lx = dy;
			ly = -dx;
		}
		
		public void updateLen()
		{
			len = Fixed64.Sqrt(vx * vx + vy * vy);
		}

		/*public void holdVector()
		{
			//reset object to other side if gone out of stage
			if (p1.x > Config.tableWidth - Config.ballRadPx)
			{
				p1.x = Config.tableWidth - Config.ballRadPx;
				vx = -Math.abs(vx);
			}
			else if	(p1.x <  Config.ballRadPx)
			{
				p1.x = Config.ballRadPx;
				vx = Math.abs(vx);
			}
			if (p1.y > Config.tableHeight - Config.ballRadPx)
			{
				p1.y = Config.tableHeight - Config.ballRadPx;
				vy = -Math.abs(vy);
			} 
			else if (p1.y < Config.ballRadPx)
			{
				p1.y = Config.ballRadPx;
				vy = Math.abs(vy);
			}
		}*/
		
		
		public string getString()
		{
			return "p0X:" + p0.x + " p0Y:" + p0.y + " p1X:" + p1.x + " p1Y:" + p1.y + " VX:" + vx + " VY:" + vy;
		}
		
		///////////////////////*******   STATIC:   ******/////////////////////////////
		
		//скалярное произведение
		public static Fixed64 getDotP(MyVector v1, MyVector v2)
		{
			return v1.vx * v2.vx + v1.vy * v2.vy;
		}
		
		
		
		private static Fixed64 dp;
		private static ProjectResult projRes;
		/**
		 * проекция вектора v1 на единичный вектор dx/dy
		 * возвращает объект с компонентами проекции
		 * @param	v1
		 * @param	dx
		 * @param	dy
		 * @return
		 */
		public static ProjectResult projectVector(MyVector v1, Fixed64 dx, Fixed64 dy)
		{
			//find dot product
			dp = v1.vx*dx + v1.vy*dy;
			projRes = new ProjectResult();
			//projection components
			projRes.vx = dp * dx;
			projRes.vy = dp * dy;
			projRes.len = Fixed64.Sqrt(projRes.vx * projRes.vx + projRes.vy * projRes.vy);
			return projRes;
		}
		
		
		private static ProjectResult proj11;
		private static ProjectResult proj12;
		private static ProjectResult proj21;
		private static ProjectResult proj22;
		private static Bounce2BallsResult bounce2ballsRes;
		private static BounceBallResult bounceBallRes;

		private static MyVector bouncePowerV = new MyVector();

		//find new movement vector bouncing from v
		public static Bounce2BallsResult bounceBalls(MyVector v1, MyVector v2, MyVector v)
		{
			//projection of v1 on v
			proj11 = projectVector(v1, v.dx, v.dy);
			//projection of v1 on v normal
			proj12 = projectVector(v1, v.lx, v.ly);
			//projection of v2 on v
			proj21 = projectVector(v2, v.dx, v.dy);
			//projection of v2 on v normal
			proj22 = projectVector(v2, v.lx, v.ly);

			//add the projections for v1
			bounce2ballsRes.vx1 = proj12.vx + proj21.vx;
			bounce2ballsRes.vy1 = proj12.vy + proj21.vy;
			//add the projections for v2
			bounce2ballsRes.vx2 = proj22.vx + proj11.vx;
			bounce2ballsRes.vy2 = proj22.vy + proj11.vy;

			bouncePowerV.vx = v1.vx - bounce2ballsRes.vx1;
			bouncePowerV.vy = v1.vy - bounce2ballsRes.vy1;
			bouncePowerV.updateLen();
			bounce2ballsRes.power = bouncePowerV.len / Config.MAX_SHOT_POWER;

			return bounce2ballsRes;
		}
		
		//v1 - шар, v2 - стена
		public static BounceBallResult bounceBallFromWall(MyVector v1, MyVector v2)
		{
			//projection of v1 on v2
			proj11 = projectVector(v1, v2.dx, v2.dy);
			//projection of v1 on v2 normal
			proj12 = projectVector(v1, v2.lx, v2.ly);
			//reverse projection on v2 normal
			proj12.len = Fixed64.Sqrt(proj12.vx * proj12.vx + proj12.vy * proj12.vy);
			proj12.vx = v2.rx * proj12.len;
			proj12.vy = v2.ry * proj12.len;
			//add the projections
			/*bounceRes.vx = v1.f*v2.f*proj11.vx+v1.b*v2.b*proj12.vx;
			bounceRes.vy = v1.f*v2.f*proj11.vy+v1.b*v2.b*proj12.vy;*/

			bounceBallRes.vx = proj11.vx + proj12.vx;
			bounceBallRes.vy = proj11.vy + proj12.vy;

			bouncePowerV.vx = v1.vx - bounceBallRes.vx;
			bouncePowerV.vy = v1.vy - bounceBallRes.vy;
			bouncePowerV.updateLen();
			bounceBallRes.power = bouncePowerV.len / Config.MAX_SHOT_POWER;

			return bounceBallRes;
		}
		
		//find new movement vector bouncing from v
		public static BounceBallResult bounceBallFromAngle(MyVector v1, MyVector vc)
		{
			//trace ("v1:" + v1.getString());
			//trace ("wall:" + vc.getString());
			//projection of v1 on v2
			proj11 = projectVector(v1, vc.lx, vc.ly);
			//projection of v1 on v2 normal
			proj12 = projectVector(v1, vc.dx, vc.dy);
			//reverse projection on v2 normal

			//trace ("proj11:" + JSON.encode(proj11))
			//trace ("proj12:" + JSON.encode(proj12))
			proj12.len = Fixed64.Sqrt(proj12.vx * proj12.vx + proj12.vy * proj12.vy);
			proj12.vx = -Fixed64.One * vc.dx * proj12.len;
			proj12.vy = -Fixed64.One * vc.dy * proj12.len;

			//trace ("   proj12:" + vk.api.serialization.json.JSON.encode(proj12))
			//add the projections
			/*bounceRes.vx = v1.f*v2.f*proj11.vx+v1.b*v2.b*proj12.vx;
			bounceRes.vy = v1.f*v2.f*proj11.vy+v1.b*v2.b*proj12.vy;*/

			bounceBallRes.vx = proj11.vx + proj12.vx;
			bounceBallRes.vy = proj11.vy + proj12.vy;

			bouncePowerV.vx = v1.vx - bounceBallRes.vx;
			bouncePowerV.vy = v1.vy - bounceBallRes.vy;
			bouncePowerV.updateLen();
			bounceBallRes.power = bouncePowerV.len / Config.MAX_SHOT_POWER;


			return bounceBallRes;
		}
	}
}
