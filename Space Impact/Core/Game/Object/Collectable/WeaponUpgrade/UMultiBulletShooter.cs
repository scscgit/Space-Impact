using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Space_Impact.Core.Game.Weapon;
using Space_Impact.Support;
using Space_Impact.Graphics;

namespace Space_Impact.Core.Game.Object.Collectable.WeaponUpgrade
{
	public class UMultiBulletShooter : AbstractWeaponUpgrade
	{
		public UMultiBulletShooter() : base("MultiBulletShooter Upgrade")
		{
			Weapon = new MultiBulletShooter(multiShot: Utility.RandomBetween(2, 5), dispersion: Utility.RandomBetween(8, 20));
			Animation = TextureSetLoader.FIRE;
		}
	}
}
