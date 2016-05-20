using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Space_Impact.Support;
using Space_Impact.Core.Game.Player.Bullet;
using Microsoft.Graphics.Canvas.Effects;
using Space_Impact.Core.Game.ActorStrategy;

namespace Space_Impact.Core.Game.Player
{
	public class Hero : AbstractPlayer, IAngle, IClickable
	{
		//Hero's thrust, class definion
		public class MovementThrust : AbstractPartActor
		{
			//Ower of the Thrust
			Hero Player;

			//Variables
			bool blinkState = false;
			int blinkCounter = 0;
			int blinkPeriod = 5;

			public MovementThrust(Hero player) : base(player.Name + "'s Thrust")
			{
				Animation = TextureSetLoader.SHIP1_THRUST;
				Player = player;
				RegisterPartIn(player);
			}

			protected override void DrawModification(ref ICanvasImage bitmap, CanvasDrawingSession draw)
			{
				if (++blinkCounter > blinkPeriod)
				{
					blinkCounter = 0;
					blinkState = !blinkState;
					if (!blinkState)
					{
						bitmap = null;
						return;
					}
				}

				//Uses same rotation as Player
				Player.RotationStrategy.DrawModification(ref bitmap, draw);
			}
		}

		//Hero's thrust
		MovementThrust thrust;
		int temporary_log_counter = 0;

		//Access to hero's strategy, should be only used for DrawModification purposes of other classes because Act() would collide
		public Rotation RotationStrategy
		{
			get; private set;
		}

		//Clickable implementation manager
		IClickable Clickable;

		public float Angle
		{
			get; set;
		}

		public Hero() : base("Hero")
		{
			Animation = TextureSetLoader.SHIP1_BASE;
			Speed = 8;
			ShootingInterval = 50;
			RotationStrategy = new Rotation(this, 10, 65);

			Clickable = new ClickableImpl(CollidesOn);

			//Hero has his thrust object
			this.thrust = new MovementThrust(this);

			//He is composed of the thrust
			ActorComposition.AddLast(this.thrust);
		}

		//Fired when player is shooting, limited by the ShootingInterval property.
		//return: true if shot was successful
		public override bool Shot()
		{
			HeroBullet bullet = new HeroBullet(this, SpaceDirection.get(SpaceDirection.VerticalDirection.UP));
			Log.i(this, "new Bullet created");

			Field.AddActor(bullet);
			return true;
		}

		public override void Act()
		{
			base.Act();

			//Calculates next expected rotation angle values
			RotationStrategy.Act();

			//Periodical logging of a Hero state
			if (temporary_log_counter < 100)
			{
				temporary_log_counter++;
			}
			else
			{
				Log.i(this, "X=" + X + " Y=" + Y +
					" Angle=" + Angle +
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
		protected override void DrawModification(ref ICanvasImage bitmap, CanvasDrawingSession draw)
		{
			//Rotates the player in the direction where he is moving
			RotationStrategy.DrawModification(ref bitmap, draw);
		}
		protected override void DrawHook(CanvasDrawingSession draw)
		{

		}

		public void Click(float x, float y)
		{
			Clickable.Click(x, y);
		}
		public void ClickMove(float x, float y)
		{
			Clickable.ClickMove(x, y);
		}
		public void ClickRelease()
		{
			Clickable.ClickRelease();
		}
	}
}
