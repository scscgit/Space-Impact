using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Services
{
	public static class PlayerController
	{
		/// <summary>
		/// Currently selected Player.
		/// </summary>
		public static Entity.Player Player
		{
			get; set;
		} = null;

		private static Dictionary<int, int> ScoreCache = new Dictionary<int, int>();
		private static object DatabaseLock = new object();

		//It seems to be more efficient to open one connection and hold it for the duration of the match... Or not to use the database during gameplay at all
		//TODO: close the connection on exit? maybe static destructor?
		static Persistence DatabaseConnection = new Persistence();

		//Adds score to the current player
		public static async Task AddScore(int score)
		{
			DatabaseConnection.Scores.Add(new Entity.Score()
			{
				Player = DatabaseConnection.Players.First(p => p.Id == Player.Id),
				ScoreValue = score
			});
			Task.Run(() =>
			{
				lock (DatabaseLock)
				{
					DatabaseConnection.SaveChangesAsync();
				}
			});
			if (ScoreCache.ContainsKey(Player.Id))
			{
				ScoreCache[Player.Id] += +score;
			}
		}

		public static int CacheScore()
		{
			if (!ScoreCache.ContainsKey(Player.Id))
			{
				ScoreCache[Player.Id] = SumScore();
			}

			return ScoreCache[Player.Id];
		}

		public static int SumScore()
		{
			// Note the following hazard when the locks aren't used:
			// 'A second operation started on this context before a previous operation completed'
			lock (DatabaseLock)
			{
				int sumScore = DatabaseConnection.Scores.Sum
				(
					score => score.Player.Id == Player.Id ? score.ScoreValue : 0
				);
				return sumScore;
			}
		}
	}
}
