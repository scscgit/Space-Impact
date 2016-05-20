using Space_Impact.Core.Game.Enemy;
using Space_Impact.Core.Game.Object.Weapon;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Character.Enemy.Bomb
{
	public abstract class AbstractBomb : AbstractEnemy, IBomb, IAffectedByBombExplosion
	{
		public bool Exploding
		{
			get; private set;
		} = false;

		//Damage that the bomb deals to the enemy on explosion
		public int Damage
		{
			get; protected set;
		}

		float radiusWidth;
		public float RadiusWidth
		{
			get
			{
				return this.radiusWidth == 0 ? (float)Width : this.radiusWidth;
			}
			protected set
			{
				this.radiusWidth = value;
			}
		}
		float radiusHeight;
		public float RadiusHeight
		{
			get
			{
				return this.radiusHeight == 0 ? (float)Height : this.radiusHeight;
			}
			protected set
			{
				this.radiusHeight = value;
			}
		}

		protected AbstractBomb(string name) : base(name, score: 5)
		{
			//Default radius is the same, as the size of the bomb itself
			RadiusWidth = 0;
			RadiusHeight = 0;
		}

		public override void Act()
		{
			base.Act();

			//The radius of detection gets reduced when the bomb is damaged.
			float radiusDetectionHealthModifier = (float)(Health + (0.5 * MaxHealth)) / MaxHealth;
			if (radiusDetectionHealthModifier > 1)
			{
				radiusDetectionHealthModifier = 1;
			}

			//The radius is at least of the size of the bomb
			float radiusWidth = RadiusWidth * radiusDetectionHealthModifier;
			if (radiusWidth < Width)
			{
				radiusWidth = (float)Width;
			}
			float radiusHeight = RadiusHeight * radiusDetectionHealthModifier;
			if (radiusHeight < Height)
			{
				radiusHeight = (float)Height;
			}

			//Self-explanatory: If enemy is within the range of radius (after the detection modifier), the bomb explodes.
			if (EnemyWithinRange(RadiusWidth * radiusDetectionHealthModifier, RadiusHeight * radiusDetectionHealthModifier))
			{
				Explode();
			}
		}

		/// <summary>
		/// Overridable actions performed when the bomb has to explode.
		/// </summary>
		protected virtual void Explode()
		{
			Log.i(this, "Bomb exploding");

			//The explosion starts; if the bomb is already exploding, this function call is meaningless and duplicate.
			if (Exploding)
			{
				return;
			}
			Exploding = true;

			int affectedActors = 0;
			Field.ForEachActor<IActor>
			(
				actor =>
				{
					IAffectedByBombExplosion killable = actor as IAffectedByBombExplosion;
					if (killable == null)
					{
						return false;
					}

					//If there is a killable actor within the range of radius, he gets affected by the explosion
					if (actor.IntersectsWithin(X, RadiusWidth, Y, RadiusHeight))
					{
						killable.OnBombExplosion(this);
						affectedActors++;
					}
					return false;
				}
			);

			AfterExplosion();
			Log.i(this, "Bomb has finished the explosion, affecting " + affectedActors + " actors");
		}

		/// <summary>
		/// Overridable actions performed after explosion.
		//	By default, removes the Bomb from the Field.
		/// </summary>
		protected virtual void AfterExplosion()
		{
			RemoveFromField();
		}

		protected bool EnemyWithinRange(float radiusWidth, float radiusHeight)
		{
			bool withinRange = false;
			Field.ForEachActor<IActor>
			(
				actor =>
				{
					//Non-killable actors don't trigger the explosion
					IAffectedByBombExplosion killable = actor as IAffectedByBombExplosion;
					if (killable == null)
					{
						return false;
					}

					//The friendly fire is not wanted and thus by itself doesn't trigger the explosion
					if (actor is IEnemy)
					{
						return false;
					}

					//If there is a killable actor within range, we set the flag to true and stop searching for more
					if (actor.IntersectsWithin(X, radiusWidth, Y, radiusHeight))
					{
						withinRange = true;
						return true;
					}
					return false;
				}
			);
			return withinRange;
		}

		public override void OnDeath()
		{
			Explode();
		}

		/// <summary>
		/// The bomb explosion has a chaining effect, every other bomb (inherited from this class) also gets dealt damage.
		/// </summary>
		/// <param name="bomb">the other bomb which exploded nearby</param>
		public void OnBombExplosion(IBomb bomb)
		{
			Health -= bomb.Damage;
		}
	}
}
