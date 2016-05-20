using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Spawner.Strategy
{
	public class EveryNPercent : ISpawnerStrategy
	{
		ISpawner Spawner;
		float StartPercent;
		float IntervalPercent;

		public EveryNPercent(ISpawner spawner, float percent)
		{
			this.Spawner = spawner;
			this.IntervalPercent = percent;
			this.StartPercent = Spawner.Field.Percent;
		}

		public void Act()
		{
			if(Spawner.Field.Percent > StartPercent + IntervalPercent)
			{
				StartPercent += IntervalPercent;
				Spawner.Spawn();
			}
		}
	}
}
