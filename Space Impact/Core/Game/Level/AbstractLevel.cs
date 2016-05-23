using Space_Impact.Core.Game.Spawner;
using Space_Impact.Core.Game.Spawner.Wrapper;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Level
{
	/// <summary>
	/// Level that defines all Spawners and Actors that are to be available in a Game Round.
	/// </summary>
	public abstract class AbstractLevel
	{
		public IField Field
		{
			get; private set;
		}
		public List<ISpawner> Spawners
		{
			get; private set;
		}

		public void AddToField(IField field)
		{
			//Saving the Field so that the Level construction can use its public a accessors
			Field = field;
			Spawners = new List<ISpawner>();

			Construct();

			int spawnerCount = Spawners.Count;
			Log.i(this, "Adding " + spawnerCount.ToString() + " spawner" + (spawnerCount == 1 ? "" : "s") + " to the Field");
			foreach (ISpawner spawner in Spawners)
			{
				field.AddActor(spawner);
			}
		}

		protected abstract void Construct();

		protected void AddDualSpawner(float percentDelay, int enemies, SpawnCallbackDelegate spawnCallback)
		{
			//Half the enemies for each side
			int enemiesLeft = enemies / 2;
			int enemiesRight = enemies - enemiesLeft;

			//Adds the Spawner to the list of Spawners intended for the game round
			Spawners.Add
			(
				new DelayedStart
				(
					actsDelay: 0
					, percentDelay: percentDelay
					, spawner: new DualSymmetrySpawner
					(
						y: 0
						, remainingEnemiesLeft: enemiesLeft
						, remainingEnemiesRight: enemiesRight
						, equalSpawnCallback: spawnCallback
					)
				)
			);
		}
	}
}
