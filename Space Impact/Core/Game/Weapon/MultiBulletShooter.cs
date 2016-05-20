using Space_Impact.Core.Game.Character;
using Space_Impact.Core.Game.Player.Bullet;
using Space_Impact.Graphics;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Weapon
{
	public class MultiBulletShooter : AbstractWeapon
	{
		//Fields
		int MultiShot;
		float Dispersion;

		public MultiBulletShooter(int multiShot, float dispersion)
		{
			MultiShot = multiShot;
			Dispersion = dispersion;
		}

		public override bool Shot(ICharacter character, Position position, float angle)
		{
			//Prevention against division by zero
			if (MultiShot <= 1)
			{
				HeroBullet bullet = new HeroBullet(character, position, angle);
				character.Field.AddActor(bullet);
			}
			//When the MultiShot is an even number, neither shot can go to the original angle direction
			else
			{
				for (int shotNumber = 0; shotNumber < MultiShot; shotNumber++)
				{
					float shotAngle = angle - Dispersion / 2 + ((float)shotNumber / (MultiShot - 1)) * Dispersion;

					HeroBullet bullet = new HeroBullet(character, position, shotAngle);
					character.Field.AddActor(bullet);
				}
			}

			return true;
			//todo ammo limitation return false
		}
	}
}
