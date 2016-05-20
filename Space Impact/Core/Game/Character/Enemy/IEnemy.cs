using Space_Impact.Core.Game.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Enemy
{
	public interface IEnemy : ICharacter
	{
		/// <summary>
		/// Score awarded to Player on kill.
		/// </summary>
		int Score { get; }
	}
}
