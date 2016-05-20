using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Spawner.Strategy
{
	public class EveryNActs: ISpawnerStrategy
	{
		ISpawner Spawner;

		/// <summary>
		/// Countdown until the next spawn, automatically executes Spawn() when the time arrives.
		/// Advised access is by CountDown--.
		/// </summary>
		private int countDown;
		protected int CountDown
		{
			get
			{
				return this.countDown;
			}
			set
			{
				this.countDown = value;
				if (this.countDown <= 0)
				{
					this.countDown = Interval;
					Spawner.Spawn();
				}
			}
		}

		/// <summary>
		/// Interval between consecutive spawns.
		/// </summary>
		private int interval;
		public int Interval
		{
			get
			{
				return this.interval;
			}
			protected set
			{
				this.interval = value;
				if (CountDown > this.interval)
				{
					CountDown = this.interval;
				}
			}
		}

		public EveryNActs(ISpawner spawner, int interval)
		{
			Spawner = spawner;

			Interval = interval;
			CountDown = interval;
		}

		public void Act()
		{
			CountDown--;
		}
	}
}
