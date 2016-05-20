using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Space_Impact.Graphics;

namespace Space_Impact.Core.Game.Spawner.Wrapper
{
	/// <summary>
	/// Gift-wraps a Spawner. It's christmas time.
	/// </summary>
	public class AbstractSpawnerWrapper : ISpawner
	{
		protected ISpawner Spawner;

		public AbstractSpawnerWrapper(ISpawner spawner)
		{
			Spawner = spawner;
		}

		public virtual bool Active
		{
			get
			{
				return Spawner.Active;
			}
		}

		public virtual IField Field
		{
			get
			{
				return Spawner.Field;
			}
		}

		public virtual Position Position
		{
			get
			{
				return Spawner.Position;
			}
		}

		public virtual int RemainingEnemies
		{
			get
			{
				return Spawner.RemainingEnemies;
			}
		}

		public virtual bool UnlimitedEnemies
		{
			get
			{
				return Spawner.UnlimitedEnemies;
			}
		}

		public virtual float X
		{
			get
			{
				return Spawner.X;
			}

			set
			{
				Spawner.X = value;
			}
		}

		public virtual float Y
		{
			get
			{
				return Spawner.Y;
			}

			set
			{
				Spawner.Y = value;
			}
		}

		public virtual double Width
		{
			get
			{
				return Spawner.Width;
			}
		}

		public virtual double Height
		{
			get
			{
				return Spawner.Height;
			}
		}

		public virtual void Act()
		{
			Spawner.Act();
		}

		public virtual void Spawn(SpawnCallbackDelegate spawnCallback)
		{
			Spawner.Spawn(spawnCallback);
		}

		public virtual void AddedToField(IField field)
		{
			Spawner.AddedToField(field);
		}

		public virtual void AddedToFieldHook()
		{
			Spawner.AddedToFieldHook();
		}

		public virtual void DeleteActor()
		{
			Spawner.DeleteActor();
		}

		public virtual void DeleteActorHook()
		{
			Spawner.DeleteActorHook();
		}
	}
}
