using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Support
{
	/// <summary>
	/// Shamelessly stealing ideas from Android.
	/// Provides single point of enabling/disabling those messages, and/or forwarding them to screen when I reimplement it.
	/// </summary>
	static class Log
	{
		//Debug - This should be used if the text is not descriptive at all and should be removed after some testing.
		public static void d(Object context, String message)
		{
			Debug.WriteLine("D>" + context.ToString() + ": " + message);
		}
		
		//Information - This message provides a control flow information and can be used for filtering, should not be displayed in Release version.
		public static void i(Object context, String message)
		{
			Debug.WriteLine("I>"+context.ToString() + ": " + message);
		}

		//Error - This represents situation that should actually crash the program, so it has to be fixed later.
		//When the game is distributed, this is a useful information to return to the developer.
		public static void e(Object context, String message)
		{
			Debug.WriteLine("E>"+context.ToString() + ": " + message);
		}
	}
}
