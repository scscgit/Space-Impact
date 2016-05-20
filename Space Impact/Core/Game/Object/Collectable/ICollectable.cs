using Space_Impact.Core.Game.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Object.Collectable
{
	public interface ICollectable : IObject
	{
		/// <summary>
		/// Object gets collected by a player.
		/// </summary>
		/// <param name="player">Player who picked the collectable object</param>
		void CollectBy(IPlayer player);
	}
}
