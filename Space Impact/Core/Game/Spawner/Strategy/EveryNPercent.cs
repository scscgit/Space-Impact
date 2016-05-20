using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Spawner.Strategy
{
	public class EveryNPercent : AbstractSpawnerStrategy
	{
		float StartPercent;
		float IntervalPercent;

		public EveryNPercent(ISpawner spawner, SpawnCallbackDelegate spawnCallback, float percent): base(spawner, spawnCallback)
		{
			this.IntervalPercent = percent;
			this.StartPercent = Spawner.Field.Percent;
		}

		public override void Act()
		{
			base.Act();

			if(Spawner.Field.Percent > StartPercent + IntervalPercent)
			{
				StartPercent += IntervalPercent;
				Spawn();
			}
		}
	}
}
