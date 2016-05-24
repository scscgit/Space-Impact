using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.ActorStrategy
{
	public delegate void StrategyCallback();

	public class EveryNActs : IActStrategy
	{
		//Currently chosen callback to be executed on success of conditions
		StrategyCallback StrategyCallback;

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
					StrategyCallback();
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

		public EveryNActs(StrategyCallback callback, int interval)
		{
			StrategyCallback = callback;
			Interval = interval;
			CountDown = interval;
		}

		public void Act()
		{
			CountDown--;
		}
	}
}
