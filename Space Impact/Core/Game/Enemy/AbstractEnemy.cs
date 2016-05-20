using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Enemy
{
	public abstract class AbstractEnemy: AbstractActor, IEnemy
	{
		public AbstractEnemy(string name): base(name)
		{

		}
	}
}
