using Space_Impact.Core.Game.Character;
using Space_Impact.Core.Game.Character.Enemy;
using Space_Impact.Graphics;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Object.Projectile.Bullet
{
	/// <summary>
	/// A basic bullet of a true Hero.
	/// Is used to kill IEnemies.
	/// </summary>
	public class HeroBullet : FireBullet<IEnemy>
	{
		public new const int DEFAULT_SPEED = 50;
		public const int DEFAULT_DAMAGE = 50;

		public HeroBullet(ICharacter player, Position position, float angle)
			: base(player, position, angle, speed: DEFAULT_SPEED, damage: DEFAULT_DAMAGE)
		{
			Name = "HeroBullet";
		}
	}
}
