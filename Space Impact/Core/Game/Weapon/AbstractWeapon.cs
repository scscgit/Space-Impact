using Space_Impact.Core.Game.Character;
using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Weapon
{
	public abstract class AbstractWeapon : IWeapon
	{
		protected AbstractWeapon()
		{
		}

		public abstract bool Shot(ICharacter character, Position position, float angle);
	}
}
