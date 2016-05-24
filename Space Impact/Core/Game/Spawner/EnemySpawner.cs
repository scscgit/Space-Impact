using Space_Impact.Core.Game.Character.Enemy;
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
		//Callback function defining the moment of next Spawn when all conditions get fulfilled
		SpawnCallbackDelegate SpawnCallback;

		public EnemySpawner(float x, float y, int remainingEnemies, SpawnCallbackDelegate spawnCallback) : base(x, y, remainingEnemies)
		{
			SpawnCallback = spawnCallback;
		}

		public override void AddedToFieldHook()
		{
			base.AddedToFieldHook();

			//Strategies used within the Spawner
			Strategies.Add(new EveryNActs(this, SpawnCallback, interval: 150));
			//Strategies.Add(new EveryNPercent(this, SpawnCallback, percent: 1));
		}
	}
}
