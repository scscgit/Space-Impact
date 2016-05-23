using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Spawner.Strategy
{
	/// <summary>
	/// Strategy of a Spawner.
	/// Should be only created on a Spawner that is already added to the Field.
	/// </summary>
	public abstract class AbstractSpawnerStrategy : ISpawnerStrategy
	{
		//Current spawner that is using the strategy
		protected ISpawner Spawner;

		//Callback that runs every time the conditions for the spawn get fulfilled, e.g.there are remaining enemies.
		private SpawnCallbackDelegate SpawnCallback;

		protected AbstractSpawnerStrategy(ISpawner spawner, SpawnCallbackDelegate spawnCallback)
		{
			Spawner = spawner;
			SpawnCallback = spawnCallback;
		}

		/// <summary>
		/// Spawns a new enemy with the chosen callback.
		/// </summary>
		protected void Spawn()
		{
			Spawner.Spawn(SpawnCallback);
		}

		public virtual void Act()
		{
		}
	}
}
