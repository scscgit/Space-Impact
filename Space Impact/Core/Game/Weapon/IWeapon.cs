using Space_Impact.Core.Game.Character;
using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Weapon
{
	/// <summary>
	/// A weapon, possibly collected as a IWeaponUpgrade, used by ICharacter for shooting new IProjectile objects.
	/// </summary>
	public interface IWeapon
	{
		/// <summary>
		/// Calling a single shot from the weapon.
		/// </summary>
		/// <returns>true if the shot was successful</returns>
		/// TODO params
		bool Shot(ICharacter character, Position position, float angle);
	}
}
