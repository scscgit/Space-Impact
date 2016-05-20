using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Space_Impact.Support;

namespace Space_Impact.Core.Game.Player
{
	public class Hero : AbstractPlayer
	{
		//Hero's thrust
		public class MovementThrust : AbstractPartActor
		{
			IPlayer player;

			bool blinkState = false;
			int blinkCounter = 0;
			int blinkPeriod = 5;

			public MovementThrust(IPlayer player) : base(player.Name + "'s Thrust")
			{
				setAnimation(TextureSetLoader.SHIP1_THRUST);
				this.player = player;
				RegisterPartIn(player);
			}

			protected override CanvasBitmap DrawModification(CanvasBitmap bitmap)
			{
				if (++blinkCounter > blinkPeriod)
				{
					blinkCounter = 0;
					blinkState = !blinkState;
				}

				return blinkState ? bitmap : null;
			}
		}

		MovementThrust thrust;

		public Hero() : base("Hero")
		{
			setAnimation(TextureSetLoader.SHIP1_BASE);
			Speed = 8;

			//Hero has his thrust object
			this.thrust = new MovementThrust(this);

			//He is composed of the thrust
			ActorComposition.AddLast(this.thrust);
		}

		int temporary_log_counter = 0;

		public override void Act()
		{
			base.Act();

			//Periodical logging of a Hero state
			if (temporary_log_counter < 100)
			{
				temporary_log_counter++;
			}
			else
			{
				Log.i(this, "X=" + X + " Y=" + Y +
					" Horizontal=" +
					(Direction.Horizontal == SpaceDirection.HorizontalDirection.LEFT ? "Left" : (Direction.Horizontal == SpaceDirection.HorizontalDirection.RIGHT ? "Right" : ".")) +
					" Vertical=" +
					(Direction.Vertical == SpaceDirection.VerticalDirection.UP ? "Up" : (Direction.Vertical == SpaceDirection.VerticalDirection.DOWN ? "Down" : ".")));
				temporary_log_counter = 0;
			}
		}

		public override void AddedToFieldHook()
		{
			Field.AddActor(this.thrust);
		}

		protected override void DrawHook(CanvasDrawingSession draw)
		{
			
		}
	}
}
