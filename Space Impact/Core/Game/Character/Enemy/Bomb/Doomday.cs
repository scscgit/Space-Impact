using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Space_Impact.Core.Game.ActorStrategy;

namespace Space_Impact.Core.Game.Character.Enemy.Bomb
{
	/// <summary>
	/// Doomday is a bomb that follows its enemy.
	/// </summary>
	public class Doomday : AbstractBomb, IBomb
	{
		public Doomday(IActor followTarget) : base("Doomday")
		{
			Animation = TextureSetLoader.DOOMDAY;
			AnimationSpeed = 20;
			Damage = 90;

			//This bomb has 3 times its animation height and width as its explosion radius
			RadiusHeight = 3 * (float)Height;
			RadiusWidth = 3 * (float)Width;

			//The strategy is a follower of a target chosen by the constructor parameter
			AddStrategy(new Follower(this, followTarget));
		}
	}
}
