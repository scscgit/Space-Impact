using Space_Impact.Core.Game.Player.Bullet;
using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Player.Bullet
{
	public class HeroBullet : AbstractBullet
	{
		public HeroBullet(IPlayer player, SpaceDirection direction) : base("Bullet", player, direction)
		{
			Animation = TextureSetLoader.FIRE;
			Speed = 50;
		}

		public override bool CollidesOn(int x, int y)
		{
			//todo lambda square static utility new static method
			return true;
		}
	}
}
