using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Object.Collectable.WeaponUpgrade
{
	public abstract class AbstractWeaponUpgrade : AbstractCollectable, IWeaponUpgrade
	{
		protected AbstractWeaponUpgrade(string name) : base(name)
		{

		}


	}
}
