using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Space_Impact.Services
{
	public sealed class BackgroundTask : IBackgroundTask
	{
		private static readonly string TAG = typeof(BackgroundTask).Name;

		//If you run any asynchronous code in your background task, then your background task needs to use a deferral.
		//If you don't use a deferral, then the background task process can terminate unexpectedly if the Run method completes before your asynchronous method call has completed.
		//Request the deferral in the Run method before calling the asynchronous method.
		//Save the deferral to a global variable so it can be accessed from the asynchronous method.Declare the deferral complete after the asynchronous code completes.
		BackgroundTaskDeferral TaskDeferral;

		public async void Run(IBackgroundTaskInstance taskInstance)
		{
			//Thread will run until the deferral gets marked as completed
			TaskDeferral = taskInstance.GetDeferral();

			//Running the game using SIP (Space Impact Protocol)
			Windows.System.LauncherOptions options = new Windows.System.LauncherOptions();
			//Use FamilyName of the current application in case there are more games using the same protocol (wait, what?)
			options.TargetApplicationPackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName;
			await Windows.System.Launcher.LaunchUriAsync(new Uri("spaceimpact:"), options);

			Log.d(this, "Test of logging from async task");

			//Marking async tasks as completed
			TaskDeferral.Complete();
		}

		/// <summary>
		/// Registers a new background task instance.
		/// </summary>
		/// <param name="taskName">Name of the registered task. Should be unique for every unique task.</param>
		/// <param name="timeInMinutes">Time in minutes after which the task will be run. Minimum is 15 minutes.</param>
		/// <returns>Registration object on success, null when the task is duplicate based on its name</returns>
		public static BackgroundTaskRegistration ScheduleOneTime(string taskName, uint timeInMinutes)
		{
			if (timeInMinutes < 15)
			{
				throw new ArgumentOutOfRangeException("Scheduled time must be at least 15 minutes, because UWP is bad");
			}

			//Checking for duplicate tasks
			foreach (var task in BackgroundTaskRegistration.AllTasks)
			{
				if (task.Value.Name.Equals(taskName))
				{
					Log.i(TAG, "The task " + taskName + " was already scheduled.");
					return null;
				}
			}

			var builder = new BackgroundTaskBuilder();
			builder.Name = taskName;
			builder.TaskEntryPoint = typeof(BackgroundTask).FullName;

			//Minimum is 15 minutes
			builder.SetTrigger(new TimeTrigger(timeInMinutes, true));

			//Registering the background task
			var registered = builder.Register();

			//Logging list of all scheduled tasks right after new schedule is made
			var tasks = BackgroundTaskRegistration.AllTasks;
			int i = 0;
			Log.i(TAG, "There are now " + tasks.Count.ToString() + " tasks scheduled:");
			foreach (var task in tasks)
			{
				Log.i(TAG, (i++ + 1).ToString() + " (ID " + task.Value.TaskId + ") = " + task.Value.Name);
			}

			return registered;
		}
	}
}
