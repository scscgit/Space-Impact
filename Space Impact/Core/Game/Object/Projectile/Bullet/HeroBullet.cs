using Space_Impact.Core.Game.IntersectStrategy;
using Space_Impact.Core.Game.Player.Bullet;
using Space_Impact.Graphics;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Player.Bullet
{
	/// <summary>
	/// A basic bullet of a true Hero.
	/// </summary>
	public class HeroBullet : AbstractBullet
	{
		public HeroBullet(IPlayer player, float angle) : base("Bullet", player, angle)
		{
			Animation = TextureSetLoader.FIRE;
			Speed = 50;
		}
	}
}
