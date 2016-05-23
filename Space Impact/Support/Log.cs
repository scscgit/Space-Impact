using MetroLog;
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
		static object SingletonLock = new object();
		static Log log = null;
		public static Log Instance
		{
			get
			{
				lock (SingletonLock)
				{
					if (log == null)
					{
						log = new Log();
					}
					return log;
				}
			}
		}

		//MetroLog
		private ILogger MetroLogger;

		//When an error happens, this flag gets changed to true
		public bool ErrorReceived;

		LinkedList<string> Queue = new LinkedList<string>();

		public Log()
		{
			MetroLogger = LogManagerFactory.DefaultLogManager.GetLogger<App>();
		}

		/// <summary>
		/// Logger must be initialized during the App initialization.
		/// </summary>
		public static void InitializeMetroLog()
		{
			bool debug = false;
#if DEBUG
			debug = true;
#endif

			//If debugging, everything will be logged
			if (debug || Utility.SettingsLoad<bool?>("debug") == true)
			{
				LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, new MetroLog.Targets.StreamingFileTarget());
			}

			//The GlobalCrashHandler can be used to ensure that a FATAL log entry is written if an unhandled exception occurs
			GlobalCrashHandler.Configure();
		}

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
		public static void d(object context, string message)
		{
			string debug = "D>" + context.ToString() + ": " + message;
			//Debug.WriteLine(debug);
			Instance.Queue.AddLast(debug);
			Instance.MetroLogger.Debug(debug);
		}

		//Information - This message provides a control flow information and can be used for filtering, should not be displayed in Release version.
		public static void i(object context, string message)
		{
			string information = "I>" + context.ToString() + ": " + message;
			//Debug.WriteLine(information);
			Instance.Queue.AddLast(information);
			Instance.MetroLogger.Info(information);
		}

		//Error - This represents situation that should actually crash the program, so it has to be fixed later.
		//When the game is distributed, this is a useful information to return to the developer.
		public static void e(object context, string message)
		{
			string error = "E>" + context.ToString() + ": " + message;
			//Debug.WriteLine(error);
			Instance.Queue.AddLast(error);
			Instance.ErrorReceived = true;
			Instance.MetroLogger.Error(error);
		}
	}
}
