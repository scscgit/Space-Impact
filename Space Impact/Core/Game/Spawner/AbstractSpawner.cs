using Space_Impact.Core.Game.Spawner.Strategy;
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
		public IField Field
		{
			get; protected set;
		}

		protected List<ISpawnerStrategy> Strategies;

		private int remainingEnemies;
		public int RemainingEnemies
		{
			get
			{
				return this.remainingEnemies;
			}
			protected set
			{
				this.remainingEnemies = value;
				if(value<=0)
				{
					NoRemainingEnemies();
				}
			}
		}

		public bool UnlimitedEnemies
		{
			get; protected set;
		} = false;

		public float X
		{
			get; set;
		} = 0;

		public float Y
		{
			get; set;
		} = 0;

		public double Width
		{
			get; set;
		} = 0;

		public double Height
		{
			get; set;
		} = 0;

		/// <summary>
		/// Returns a current representative target position for the Spawner.
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

		public delegate bool IsActive();
		private IsActive activeStrategy;
		/// <summary>
		/// Strategy for dynamical switching of the condition of Active state of the spawner.
		/// </summary>
		public IsActive ActiveStrategy
		{
			protected get
			{
				return this.activeStrategy;
			}
			set
			{
				this.activeStrategy = value;
			}
		}

		public bool Active
		{
			get
			{
				return ActiveStrategy();
			}
		}

		protected AbstractSpawner(IField field, float x, float y, int remainingEnemies)
		{
			Field = field;
			X = x;
			Y = y;
			Width = 0;
			Height = 0;
			RemainingEnemies = remainingEnemies;
			Strategies = new List<ISpawnerStrategy>();

			//By default, Spawner is active only while the round is not finished
			ActiveStrategy = () => Field.GameRunning;
		}

		/// <summary>
		/// Public spawn interface implementation, calls the callback of the subclasses.
		/// </summary>
		public void Spawn()
		{
			if (UnlimitedEnemies)
			{
				SpawnCallback();
			}
			else if (RemainingEnemies > 0)
			{
				SpawnCallback();
				RemainingEnemies--;
			}
		}

		/// <summary>
		/// Callback hook for event of running out of enemies.
		/// </summary>
		protected void NoRemainingEnemies()
		{
			//Default implementation removes the spawner
			Field.RemoveActor(this);
		}

		/// <summary>
		/// Callback that runs every time the conditions for the spawn get fulfilled, e.g. there are remaining enemies.
		/// </summary>
		protected abstract void SpawnCallback();

		public virtual void Act()
		{
			//Run all strategies if the Spawner is currently active
			if (Active)
			{
				foreach (ISpawnerStrategy strategy in Strategies)
				{
					strategy.Act();
				}
			}
		}
	}
}
