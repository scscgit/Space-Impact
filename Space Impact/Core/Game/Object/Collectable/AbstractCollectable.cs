using Space_Impact.Core.Game.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Object.Collectable
{
	public abstract class AbstractCollectable : AbstractObject, ICollectable
	{
		protected AbstractCollectable(string name) : base(name)
		{
			Direction = SpaceDirection.Get(SpaceDirection.HorizontalDirection.NONE, SpaceDirection.VerticalDirection.DOWN);
		}

		/// <summary>
		/// Object gets collected by a player.
		/// </summary>
		/// <param name="player">Player who picked the collectable object</param>
		public abstract void CollectBy(IPlayer player);

		public override void Act()
		{
			base.Act();
			Field.ForEachActor<IPlayer>
			(
				player =>
				{
					if (IntersectsActor(player))
					{
						CollectBy(player);
					}
					return false;
				}
			);
		}
	}
}
