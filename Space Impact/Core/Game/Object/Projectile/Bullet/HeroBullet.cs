using Space_Impact.Core.Game.Enemy;
using Space_Impact.Core.Game.IntersectStrategy;
using Space_Impact.Core.Game.Player.Bullet;
using Space_Impact.Graphics;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Player.Bullet
{
	/// <summary>
	/// A basic bullet of a true Hero.
	/// </summary>
	public class HeroBullet : AbstractBullet
	{
		public HeroBullet(IPlayer player, float angle) : base("Bullet", player, angle)
		{
			Animation = TextureSetLoader.FIRE;
			Speed = 50;
			Damage = 50;
		}

		public override void Act()
		{
			base.Act();

			List<IEnemy> hits = new List<IEnemy>();
			Field.ForEachActor<IEnemy>
			(
				enemy =>
				{
					if (IntersectsActor(enemy))
					{
						hits.Add(enemy);
					}
					return false;

					/*
					//Old implementation:
					//Expects field to take care of the removal by only acting on one enemy; no damage diminishing returns
					if (IntersectsActor(enemy))
					{
						enemy.Health -= this.Damage;
						RemoveFromField();
						return true;
					}
					return false;
					*/
				}
			);

			Log.i(this, "hits " + hits.Count.ToString());

			if (hits.Count > 0)
			{
				int damage = Damage / hits.Count;
				foreach (IEnemy enemy in hits)
				{
					enemy.Health -= damage;
				}
				RemoveFromField();
			}
		}
	}
}
