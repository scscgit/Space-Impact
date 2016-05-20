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
		public class MovementThrust : AbstractActor
		{
			IPlayer player;

			bool blinkState = false;
			int blinkCounter = 0;
			int blinkPeriod = 5;

			public MovementThrust(IPlayer player) : base(player.Name + "'s Thrust")
			{
				setAnimation(TextureSetLoader.SHIP1_THRUST);
				this.player = player;
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

			this.thrust = new MovementThrust(this);
		}

		public override void Act()
		{
			Log.i(this,"X="+X+" Y="+Y+" Horizontal="+(Direction.Horizontal==SpaceDirection.HorizontalDirection.LEFT?"L":(Direction.Horizontal == SpaceDirection.HorizontalDirection.RIGHT?"R":".")));
			base.Act();
		}

		public override void AddedToFieldHook()
		{
			Field.AddActor(this.thrust);
		}

		protected override void DrawHook(CanvasDrawingSession draw)
		{
			//thrust.Draw(draw);
		}
	}
}
