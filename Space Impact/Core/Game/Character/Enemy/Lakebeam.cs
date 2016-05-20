using Space_Impact.Core.Game.Enemy;
using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Space_Impact.Core.Game.ActorStrategy;

namespace Space_Impact.Core.Game.Character.Enemy
{
	public class Lakebeam : AbstractEnemy
	{
		public Lakebeam() : base("Lakebeam")
		{
			Animation = TextureSetLoader.SHIP2_BASE;
			Direction = SpaceDirection.None + SpaceDirection.VerticalDirection.DOWN;

			AddStrategy(new FlyingRotation(this, SpaceDirection.VerticalDirection.DOWN, 50, 120));


			Direction += SpaceDirection.HorizontalDirection.RIGHT;

		}

		protected override void DrawModification(ref ICanvasImage bitmap, CanvasDrawingSession draw)
		{
			base.DrawModification(ref bitmap, draw);
		}

		public override void Act()
		{
			base.Act();
		}

		public override void OnDeath()
		{
			//todo
			throw new NotImplementedException();
		}
	}
}
