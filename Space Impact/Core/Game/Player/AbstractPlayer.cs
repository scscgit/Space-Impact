using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Player
{
	public abstract class AbstractPlayer : AbstractActor
	{
		public AbstractPlayer()
		{

		}

		public override void Act()
		{
			X++;
		}
	}
}
