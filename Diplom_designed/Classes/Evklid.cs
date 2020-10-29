
namespace Diplom_designed
{
	class Evklid
	{
		private int _nod;
		private int _reversNumber;

		public Evklid(int a, int b)
		{
			int x = 0;
			int y = 0;
			_nod = Gcd(a, b, ref x, ref y);
			_reversNumber = x;
		}

		public int GetNod()
		{
			return _nod;
		}

		public int GetRevers()
		{
			return _reversNumber;
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
			int d = Gcd(n % a, a, ref x1, ref y1);

			x = y1 - (n / a) * x1;
			y = x1;
			return d;
		}
	}
}
