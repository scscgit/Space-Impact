using Space_Impact.Core.Game.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Enemy
{
	public abstract class AbstractEnemy: AbstractCharacter, IEnemy
	{
		protected AbstractEnemy(string name): base(name)
		{

		}
	}
}
