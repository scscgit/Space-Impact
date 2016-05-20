using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Player
{
	public class MovementFlame: AbstractActor
	{
		public MovementFlame()
		{
			setAnimation
				(
				new string[]
				{
					"ship1_fire"
				}
				);
		}
	}
}
