using Space_Impact.Core.Game.ActorStrategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Spawner.Strategy
{
	/// <summary>
	/// Simply delegates the Spawn function to the more general version of EveryNActs Strategy
	/// </summary>
	public class EveryNActs : AbstractSpawnerStrategy
	{
		private IActStrategy EveryNActsStrategy;

		public EveryNActs(ISpawner spawner, SpawnCallbackDelegate spawnCallback, int interval) : base(spawner, spawnCallback)
		{
			EveryNActsStrategy = new ActorStrategy.EveryNActs(Spawn, interval);
		}

		public override void Act()
		{
			base.Act();
			EveryNActsStrategy.Act();
		}
	}
}
