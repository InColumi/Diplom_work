using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom_designed
{
	public partial class StartMenu
	{
		bool CheckSimpleNumberMethodFerma(int x)
		{
			if(x == 2)
			{
				return true;
			}

			Random random = new Random();
			for(int i = 0; i < 100; i++)
			{
				int a = random.Next(x - 2) + 2;
				if(Nod(a, x) != 1)
					return false;
				if(FindPowsNumber(a, x - 1, x) != 1)
					return false;
			}
			return true;
		}

		int FindGCD(int a, int b)
		{
			if(b == 0)
			{
				return a;
			}

			return FindGCD(b, a % b);
		}

		int GetMul(int a, int b, int m)
		{
			if(b == 1)
			{
				return a;
			}
				
			if(b % 2 == 0)
			{
				int t = GetMul(a, b / 2, m);
				return (2 * t) % m;
			}
			return (GetMul(a, b - 1, m) + a) % m;
		}

		int FindPowsNumber(int a, int b, int m)
		{
			if(b == 0)
			{
				return 1;
			}
				
			if(b % 2 == 0)
			{
				int t = FindPowsNumber(a, b / 2, m);
				return GetMul(t, t, m) % m;
			}
			return (GetMul(FindPowsNumber(a, b - 1, m), a, m)) % m;
		}
	}
}
