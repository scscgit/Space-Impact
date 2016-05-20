using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Support
{
	//Shamelessly stealing ideas from Android.
	//Provides single point of enabling/disabling those messages, and/or forwarding them to screen when I reimplement it.
	static class Log
	{
		//Debug
		public static void d(Object context, String message)
		{
			Debug.WriteLine("D>" + context.ToString() + ": " + message);
		}

		//Information
		public static void i(Object context, String message)
		{
			Debug.WriteLine("I>"+context.ToString() + ": " + message);
		}

		//Error
		public static void e(Object context, String message)
		{
			Debug.WriteLine("E>"+context.ToString() + ": " + message);
		}
	}
}
