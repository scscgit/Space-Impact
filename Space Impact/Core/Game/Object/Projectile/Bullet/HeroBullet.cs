using Space_Impact.Core.Game.Character;
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
		public new const int DEFAULT_SPEED = 50;
		public const int DEFAULT_DAMAGE = 50;

		public HeroBullet(ICharacter player, Position position, float angle) : base("Bullet", player, position, angle)
		{
			Animation = TextureSetLoader.FIRE;
			Speed = DEFAULT_SPEED;
			Damage = DEFAULT_DAMAGE;
		}

		public override void Act()
		{
			base.Act();

			//If the bullet was removed from the Field during the movement, Act will stop
			if (Field == null)
			{
				return;
			}

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

			if (hits.Count > 0)
			{
				int damage = Damage / hits.Count;
				foreach (IEnemy enemy in hits)
				{
					enemy.Health -= damage;
				}
				RemoveFromField();
				Log.i(this, "Hits: " + hits.Count.ToString() + ", damage to each target: " + damage.ToString());
			}
		}
	}
}
