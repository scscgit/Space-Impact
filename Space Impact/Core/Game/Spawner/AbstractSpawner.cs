using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Spawner
{
	public abstract class AbstractSpawner : ISpawner, IPlacedOnField
	{
		//Countdown until the next spawn, automatically executes Spawn() when the time arrives
		//Advised access is by CountDown--.
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
				if(this.countDown <= 0)
				{
					this.countDown = Interval;
					if (RemainingEnemies > 0)
					{
						RemainingEnemies--;
						Spawn();
					}
				}
			}
		}

		//Interval between spawns
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
				if(CountDown>this.interval)
				{
					CountDown = this.interval;
				}
			}
		}

		protected IField Field
		{
			get; set;
		}

		public int RemainingEnemies
		{
			get; protected set;
		}

		public float X
		{
			get; set;
		}

		public float Y
		{
			get; set;
		}

		public double Width
		{
			get; set;
		}

		public double Height
		{
			get; set;
		}

		/// <summary>
		/// Returns a current representative target position for the Spawner
		/// </summary>
		protected Position Position
		{
			get
			{
				var pos = new Position();
				pos.X = X + (float)Width / 2;
				pos.Y = Y + (float)Height / 2;
				return pos;
			}
		}

		protected AbstractSpawner(IField field, float x, float y)
		{
			Field = field;

			CountDown = 1;
			Interval = 100;
			RemainingEnemies = 0;

			X = x;
			Y = y;
			Width = 0;
			Height = 0;
		}
		
		protected abstract void Spawn();

		public virtual void Act()
		{
			CountDown--;
		}
	}
}
