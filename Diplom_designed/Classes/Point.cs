using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom_designed
{
	public partial class StartMenu
	{
		class Point
		{
			private int _xPoint;    // координата x
			private int _yPoint;    // координата y
			private readonly bool _infinityZero;

			public Point()
			{
				_xPoint = 0;
				_yPoint = 0;
				_infinityZero = true;
			}

			public Point(bool zero)
			{
				_xPoint = 0;
				_yPoint = 0;
				_infinityZero = zero;
			}

			public Point(int x, int y)
			{
				_xPoint = x;
				_yPoint = y;
				_infinityZero = false;
			}

			public static Point operator +(Point p1, Point p2)
			{
				Point ans = new Point(false);
				int lamda;
				if(p1._infinityZero)
					return p2;
				else if(p2._infinityZero)
					return p1;
				else if(p1._xPoint == p2._xPoint && p1._yPoint == Mod(-p2._yPoint, _global_modP))
					return new Point(true);
				else if(p1._xPoint == p2._xPoint && p1._yPoint == p2._yPoint)
				{
					Evklid number = new Evklid(Mod(2 * p1._yPoint, _global_modP), _global_modP);
					if(number.GetNod() != 1)
						throw new Exception("Неподходящие параметры для шифрования: НОД не равен 1.");
					else
						lamda = Mod((3 * p1._xPoint * p1._xPoint + _global_b) * number.GetRevers(), _global_modP);
				}
				else
				{
					Evklid number = new Evklid(Mod(p2._xPoint - p1._xPoint, _global_modP), _global_modP);
					if(number.GetNod() != 1)
						throw new Exception("Неподходящие параметры для шифрования: НОД не равен 1.");
					else
						lamda = Mod((p2._yPoint - p1._yPoint) * number.GetRevers(), _global_modP);
				}
				ans._xPoint = Mod(lamda * lamda - p1._xPoint - p2._xPoint, _global_modP);
				ans._yPoint = Mod(lamda * (p1._xPoint - ans._xPoint) - p1._yPoint, _global_modP);
				return ans;
			}

			public static Point operator -(Point p1, Point p2)
			{
				if(p1._infinityZero)
				{
					return p1;
				}
				else
				{
					Point pTemp = new Point(false)
					{
						_xPoint = p2._xPoint,
						_yPoint = Mod(-p2._yPoint, _global_modP)
					};
					return p1 + pTemp;
				}
			}

			public static Point operator *(Point p1, int num)
			{
				Point answer = p1;
				for(int i = 0; i < num - 1; i++)
					answer += p1;
				return answer;
			}

			public override string ToString()
			{
				return $"{_xPoint},{_yPoint},";
			}

			public int GetX()
			{
				return _xPoint;
			}

			public int GetY()
			{
				return _yPoint;
			}

			public bool CheckInfinityZero()
			{
				return _infinityZero;
			}
		}
	}
}
