using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Services
{
	public class ScoreService
	{
		public readonly string QUERY_SELECT = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

		private static object ConstructorLock = new object();

		private static ScoreService instance = null;
		public static ScoreService Instance
		{
			get
			{
				lock (ConstructorLock)
				{
					if (instance == null)
					{
						instance = new ScoreService();
					}
					return instance;
				}
			}
		}

		public void Connect()
		{
			//todo
			//System.Data.S
			//SQLiteConnection
			//SqlConnection
			//SQLite
			

			//System.Data.S
		}
	}
}
