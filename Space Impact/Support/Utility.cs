using Space_Impact.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Space_Impact.Support
{
	public static class Utility
	{
		private static Random Random = new Random();

		/// <summary>
		/// Safely exists the Game, logging and flushing MetroLog in the process.
		/// </summary>
		/// <param name="context">object which requested the game exit, will be converted using ToString() for logging purposes</param>
		public async static void ExitGame(object context)
		{
			Log.i(context, "Exiting Game");
			await MetroLog.LazyFlushManager.FlushAllAsync(new MetroLog.LogWriteContext());
			Windows.UI.Xaml.Application.Current.Exit();
		}

		public static float NormalizeDegreeAngle(float angle)
		{
			angle %= 360;
			if (angle < 0)
			{
				angle += 360;
			}
			return angle;
		}

		public static float NormalizeRadianAngle(float angle)
		{
			float threesixty = (float)Math.PI * 2;
			angle %= threesixty;
			if (angle < 0)
			{
				angle += threesixty;
			}
			return angle;
		}

		/// <summary>
		/// Loads an instance of the Media Player for music, that can be accessed via .Play() and .Stop().
		/// </summary>
		/// <param name="additionalPath">path to the location folder of the mp3 file on top of the Assets\Music\</param>
		/// <param name="mp3">filename of the music without .mp3 postfix</param>
		/// <returns>Instance of the MP3 Music Player</returns>
		public async static Task<MediaElement> GetMusic(string additionalPath, string mp3)
		{
			//Initializing Audio
			Windows.Storage.StorageFolder folder;
			Windows.Storage.StorageFile musicFile;
			string path = @"Assets\Music";
			if (additionalPath != null)
			{
				path += @"\" + additionalPath;
			}
			folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(path);
			musicFile = await folder.GetFileAsync(mp3 + ".mp3");
			MediaElement music = new MediaElement();
			music.SetSource(await musicFile.OpenAsync(Windows.Storage.FileAccessMode.Read), musicFile.ContentType);
			return music;
		}
		public async static Task<MediaElement> GetMusic(string mp3)
		{
			return await GetMusic(null, mp3);
		}

		//Improvement over Math.pow() and less prone to programming mistakes than multiplying directly imo
		public static double square(double number)
		{
			return number * number;
		}
		public static float square(float number)
		{
			return number * number;
		}

		/// <summary>
		/// Returns a number between (inclusively) the two numbers.
		/// </summary>
		/// <param name="a">first bound</param>
		/// <param name="b">second bound</param>
		/// <returns></returns>
		public static int RandomBetween(int a, int b)
		{
			if (a <= b)
			{
				return Random.Next(a, b + 1);
			}
			else
			{
				return Random.Next(b, a + 1);
			}
		}

		public static Windows.Storage.ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
		public static Windows.Storage.StorageFolder LocalFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

		/// <summary>
		/// Loads a setting from the local configuration.
		/// </summary>
		/// <typeparam name="T">Type of the setting</typeparam>
		/// <param name="name">Name of the setting, that was used when saving</param>
		/// <returns>Setting with the type T</returns>
		public static T SettingsLoad<T>(string name)
		{
			object value = LocalSettings.Values[name];
			if (value is T)
			{
				return (T)value;
			}
			else
			{
				return default(T);
			}
		}

		/// <summary>
		/// Stores a setting to the local configuration
		/// </summary>
		/// <typeparam name="T">Type of the setting</typeparam>
		/// <param name="name">Name of the setting, that will be used when loading</param>
		/// <param name="value">Setting value with the type T</param>
		public static void SettingsSave<T>(string name, T value)
		{
			LocalSettings.Values[name] = value;
		}
	}
}
