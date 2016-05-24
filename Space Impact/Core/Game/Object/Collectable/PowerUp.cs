using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Space_Impact.Core.Game.Player;
using Space_Impact.Support;
using Space_Impact.Graphics;

namespace Space_Impact.Core.Game.Object.Collectable
{
	public class PowerUp : AbstractCollectable
	{
		public int HealthBoost
		{
			get; private set;
		}

		public PowerUp() : base("PowerUp")
		{
			Animation = TextureSetLoader.HOT_SIDE_OBJECT;

			//Random health bonus
			HealthBoost = Utility.RandomBetween(150, 350);
		}

		public override void CollectBy(IPlayer player)
		{
			if (player.Health < player.MaxHealth)
			{
				player.Health += HealthBoost;
				RemoveFromField();
			}
		}
	}
}
