using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Object.Weapon
{
	public interface IProjectile : IObject, IAngle
	{
		int Damage
		{
			get;
		}
	}
}
