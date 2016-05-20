using Space_Impact.Core;
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
	class Log
	{
		//Singleton
		static Log log = null;
		public static Log Instance
		{
			get
			{
				if (log == null) log = new Log();
				return log;
			}
		}

		LinkedList<string> Queue = new LinkedList<string>();

		//Used to take a next message from the queue (before some kind of output)
		public static string NextMessage
		{
			get
			{
				if (Instance.Queue.Count == 0)
				{
					return null;
				}
				string message = Instance.Queue.First.Value;
				Instance.Queue.RemoveFirst();
				return message;
			}
		}

		//Debug - This should be used if the text is not descriptive at all and should be removed after some testing.
		public static void d(Object context, String message)
		{
			string debug = "D>" + context.ToString() + ": " + message;
			Debug.WriteLine(debug);
			Instance.Queue.AddLast(debug);
		}

		//Information - This message provides a control flow information and can be used for filtering, should not be displayed in Release version.
		public static void i(Object context, String message)
		{
			string debug = "I>" + context.ToString() + ": " + message;
			Debug.WriteLine(debug);
			Instance.Queue.AddLast(debug);
		}

		//Error - This represents situation that should actually crash the program, so it has to be fixed later.
		//When the game is distributed, this is a useful information to return to the developer.
		public static void e(Object context, String message)
		{
			string debug = "E>" + context.ToString() + ": " + message;
			Debug.WriteLine(debug);
			Instance.Queue.AddLast(debug);
		}
	}
}
