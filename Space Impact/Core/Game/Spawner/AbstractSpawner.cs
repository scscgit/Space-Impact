using Space_Impact.Core.Game.Spawner.Strategy;
using Space_Impact.Graphics;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Spawner
{
	public abstract class AbstractSpawner : ISpawner, IPlacedInSpace, IPlacedInField
	{
		public IField Field
		{
			get; private set;
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
				if (value <= 0)
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

		public Position Position
		{
			get
			{
				var pos = new Position();
				pos.X = X + (float)Width / 2;
				pos.Y = Y + (float)Height / 2;
				return pos;
			}
		}

		public delegate bool IsActiveDelegate();
		private IsActiveDelegate isActiveStrategy;
		/// <summary>
		/// Strategy for dynamical switching of the condition of Active state of the spawner.
		/// </summary>
		public IsActiveDelegate IsActiveStrategy
		{
			protected get
			{
				return this.isActiveStrategy;
			}
			set
			{
				this.isActiveStrategy = value;
			}
		}

		public bool Active
		{
			get
			{
				return IsActiveStrategy();
			}
		}

		protected AbstractSpawner(float x, float y, int remainingEnemies)
		{
			X = x;
			Y = y;
			Width = 0;
			Height = 0;
			RemainingEnemies = remainingEnemies;
			Strategies = new List<ISpawnerStrategy>();

			//By default, Spawner is active only while the round is not finished
			IsActiveStrategy = () => Field.GameRunning;
		}

		/// <summary>
		/// Public spawn interface implementation, calls the callback of the subclasses.
		/// </summary>
		/// <param name="spawnCallback">Callback that runs if the conditions for the spawn get fulfilled, e.g. there are remaining enemies.</param>
		public void Spawn(SpawnCallbackDelegate spawnCallback)
		{
			if (UnlimitedEnemies)
			{
				spawnCallback();
			}
			else if (RemainingEnemies > 0)
			{
				spawnCallback();
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

		public void AddedToField(IField field)
		{
			DeleteActor();
			Field = field;
			AddedToFieldHook();
		}
		public virtual void AddedToFieldHook()
		{
		}

		public void DeleteActor()
		{
			if (Field != null)
			{
				Field.RemoveActor(this);
				Field = null;
				DeleteActorHook();
				Log.i(this, "Removed actor");
			}
		}
		public virtual void DeleteActorHook()
		{
		}
	}
}
