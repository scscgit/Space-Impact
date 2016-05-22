using Microsoft.EntityFrameworkCore;
using Space_Impact.Services.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Services
{
	/// <summary>
	/// Persists Entities in the Database, just like JPA.
	/// </summary>
	public class Persistence : DbContext
	{
		//Configuration
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			//Database will be using SQLite
			optionsBuilder.UseSqlite("Filename=Score Database.db");
		}

		/// <summary>
		/// Access to persisting Players.
		/// </summary>
		public DbSet<Player> Players { get; set; }

		/// <summary>
		/// Access to persisting Scores.
		/// </summary>
		public DbSet<Score> Scores { get; set; }
	}
}
