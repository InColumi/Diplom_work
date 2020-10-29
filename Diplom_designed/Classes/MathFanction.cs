
namespace Diplom_designed
{
	public partial class StartMenu
	{
		private static int Mod(int a, int p)
		{
			if(a >= 0)
				return a % p;
			else
				return p + (a % p);
		}

		private int ReversNumber(int a, int n)
		{
			int x = 0;
			int y = 0;
			Gcd(a, n, ref x, ref y);
			return x;
		}

		private int Gcd(int a, int n, ref int x, ref int y)
		{
			if(a == 0)
			{
				x = 0;
				y = 1;
				return n;
			}
			int x1 = 0;
			int y1 = 0;
			int d;
			d = Gcd(n % a, a, ref x1, ref y1);
			x = y1 - (n / a) * x1;
			y = x1;
			return d;
		}

		private int Nod(int a, int b)
		{
			if(a == b)
			{
				return a;
			}
				
			if(a > b)
			{
				int tmp = a;
				a = b;
				b = tmp;
			}

			return Nod(a, b - a);
		}
	}
}