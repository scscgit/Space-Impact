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
		public HeroBullet(IPlayer player, float angle) : base("Bullet", player, angle)
		{
			Animation = TextureSetLoader.FIRE;
			Speed = 50;
		}

		public override bool IntersectsOn(float x, float y)
		{
			//todo lambda square static utility new static method
			return true;
		}
	}
}
