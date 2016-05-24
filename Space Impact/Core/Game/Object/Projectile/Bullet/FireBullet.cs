using Space_Impact.Core.Game.Character;
using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Object.Projectile.Bullet
{
	/// <summary>
	/// Generic Fire Bullet used to kill any type of Enemy.
	/// </summary>
	/// <typeparam name="Enemy">Bullet deals damage to this kind of Characters.</typeparam>
	public class FireBullet<Enemy> : AbstractBullet where Enemy : ICharacter
	{
		public FireBullet(ICharacter owner, Position position, float angle, int speed, int damage)
			: base("FireBullet", owner, position, angle)
		{
			Animation = TextureSetLoader.FIRE;
			Speed = speed;
			Damage = damage;
		}

		public override void Act()
		{
			base.Act();

			//If the bullet was removed from the Field during the movement, the Act will stop
			if (Field == null)
			{
				return;
			}

			List<Enemy> hits = new List<Enemy>();
			Field.ForEachActor<Enemy>
			(
				enemy =>
				{
					if (IntersectsActor(enemy))
					{
						hits.Add(enemy);
					}
					return false;
				}
			);

			if (hits.Count > 0)
			{
				int damage = Damage / hits.Count;
				foreach (Enemy enemy in hits)
				{
					enemy.Health -= damage;
				}
				RemoveFromField();
			}
		}
	}
}
