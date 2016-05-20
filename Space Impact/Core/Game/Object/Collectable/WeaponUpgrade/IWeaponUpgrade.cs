using Space_Impact.Core.Game.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Object.Collectable.WeaponUpgrade
{
	public interface IWeaponUpgrade : ICollectable
	{
		IWeapon Weapon { get; }
	}
}
