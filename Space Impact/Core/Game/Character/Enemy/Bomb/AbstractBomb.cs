using Space_Impact.Core.Game.Enemy;
using Space_Impact.Core.Game.Object.Weapon;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Object.Bomb
{
	public abstract class AbstractBomb : AbstractEnemy, IBomb
	{
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

		protected AbstractBomb(string name) : base(name)
		{
			//Default radius is the same, as the size of the bomb itself
			RadiusWidth = 0;
			RadiusHeight = 0;
		}

		public override void Act()
		{
			base.Act();

			//Self-explanatory: If enemy is within the range of radius width and radius height, the bomb explodes.
			if (EnemyWithinRange(RadiusWidth, RadiusHeight))
			{
				Explode();
			}
		}

		//Overridable actions performed when the bomb has to explode
		protected virtual void Explode()
		{
			Log.i(this, "Bomb exploding");
			int affectedActors = 0;
			Field.ForEachActor
			(
				actor =>
				{
					IAffectedByBombExplosion killable = actor as IAffectedByBombExplosion;
					if (killable == null)
					{
						return false;
					}

					//If there is a killable actor within range, we set the flag to true and stop searching for more
					if (actor.IntersectsWithin(X, RadiusWidth, Y, RadiusHeight))
					{
						killable.OnBombExplosion(this);
						affectedActors++;
					}
					else
					{
						Log.d(this,"Actor "+actor+" not affected by explosion on X="+X+" Y="+Y+" (Bomb "+actor.X+"x"+actor.Y+")");
					}
					return false;
				}
			);

			AfterExplosion();
			Log.i(this, "Bomb has finished the explosion, affecting " + affectedActors + " actors");
		}

		//Overridable actions performed after explosion, by default, removes the bomb from the field
		protected virtual void AfterExplosion()
		{
			RemoveFromField();
		}

		protected bool EnemyWithinRange(float radiusWidth, float radiusHeight)
		{
			bool withinRange = false;
			Field.ForEachActor
			(
				actor =>
				{
					IAffectedByBombExplosion killable = actor as IAffectedByBombExplosion;
					if (killable == null)
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
	}
}
