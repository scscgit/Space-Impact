using Space_Impact.Core.Game.Character;
using Space_Impact.Core.Game.Object.Weapon;
using Space_Impact.Graphics;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Player.Bullet
{
	public abstract class AbstractBullet : AbstractProjectile, IBullet
	{
		protected AbstractBullet(string name, ICharacter character, Position position, float angle) : base(name, character, position, angle)
		{
		}
	}
}
