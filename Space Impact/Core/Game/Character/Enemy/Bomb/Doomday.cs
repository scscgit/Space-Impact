using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Space_Impact.Core.Game.ActorStrategy;
using Space_Impact.Core.Game.Object.Bomb;

namespace Space_Impact.Core.Game.Object
{
	/// <summary>
	/// Doomday is a bomb that follows its enemy.
	/// </summary>
	public class Doomday : AbstractBomb, IBomb
	{
		//Current Strategy for Act
		IActStrategy ActStrategy;

		public Doomday(IActor followTarget) : base("Doomday")
		{
			Animation = TextureSetLoader.DOOMDAY;
			AnimationSpeed = 20;
			Damage = 96;

			//The strategy is a follower of a target chosen by the constructor parameter
			ActStrategy = new Follower(this, followTarget);
		}

		//protected override void DrawModification(ref ICanvasImage bitmap, CanvasDrawingSession draw)
		//{
		//	if(IntersectsActor(Field.Player))
		//	{
		//		bitmap = null;
		//	}
		//}

		public override void Act()
		{
			base.Act();
			ActStrategy.Act();
		}

		public override void OnDeath()
		{
			//todo
			Explode();
		}
	}
}
