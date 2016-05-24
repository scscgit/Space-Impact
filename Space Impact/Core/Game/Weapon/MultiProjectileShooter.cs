using Space_Impact.Core.Game.Character;
using Space_Impact.Core.Game.Object.Projectile;
using Space_Impact.Core.Game.Object.Projectile.Bullet;
using Space_Impact.Graphics;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Weapon
{
	public delegate IProjectile NewProjectileCallback(ICharacter character, Position position, float angle);

	public class MultiProjectileShooter : AbstractWeapon
	{
		//Fields
		int MultiShot;
		float Dispersion;

		NewProjectileCallback NewProjectileCallback;

		public MultiProjectileShooter(int multiShot, float dispersion, NewProjectileCallback newProjectileCallback)
		{
			MultiShot = multiShot;
			Dispersion = dispersion;

			NewProjectileCallback = newProjectileCallback;
		}

		public override bool Shot(ICharacter character, Position position, float angle)
		{
			//Prevention against division by zero
			if (MultiShot <= 1)
			{
				IProjectile projectile = NewProjectileCallback(character, position, angle);
				character.Field.AddActor(projectile);
			}
			//When the MultiShot is an even number, neither shot can go to the original angle direction
			else
			{
				for (int shotNumber = 0; shotNumber < MultiShot; shotNumber++)
				{
					float shotAngle = angle - Dispersion / 2 + ((float)shotNumber / (MultiShot - 1)) * Dispersion;

					IProjectile projectile = NewProjectileCallback(character, position, shotAngle);
					character.Field.AddActor(projectile);
				}
			}

			return true;
			//todo ammo limitation return false
		}
	}
}
