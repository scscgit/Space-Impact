using Space_Impact.Core.Game.Character.Enemy;
using Space_Impact.Core.Game.Enemy;
using Space_Impact.Core.Game.Object;
using Space_Impact.Core.Game.Spawner.Strategy;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Spawner
{
	public class EnemySpawner : AbstractSpawner
	{
		SpawnCallbackDelegate SpawnCallback;

		public EnemySpawner(float x, float y, int remainingEnemies, SpawnCallbackDelegate spawnCallback) : base(x, y, remainingEnemies)
		{
			SpawnCallback = spawnCallback;
		}

		public override void AddedToFieldHook()
		{
			base.AddedToFieldHook();

			//Strategies used within the Spawner
			Strategies.Add(new EveryNActs(this, SpawnCallback, interval: 80));
			//Strategies.Add(new EveryNPercent(this, SpawnCallback, percent: 1));
		}

		public override void DeleteActorHook()
		{
			base.DeleteActorHook();

			//Delete strategies from the Spawner
			Log.i(this, "Clearing strategies");
			Strategies.Clear();
		}

		public override void Act()
		{
			base.Act();
		}
	}
}
