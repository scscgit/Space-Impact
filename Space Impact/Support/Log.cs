using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Support
{
	static class Log
	{
		public static void i(String message)
		{
			Debug.WriteLine(message);
		}
	}
}
