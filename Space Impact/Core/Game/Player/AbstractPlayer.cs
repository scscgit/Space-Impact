using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Player
{
	public abstract class AbstractPlayer : AbstractActor, IPlayer
	{
		public AbstractPlayer(string name) : base(name)
		{
		}

		public override void Act()
		{
			base.Act();
		}
	}
}
