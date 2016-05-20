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
using Space_Impact.Core.Game.Object.Bomb;
using Space_Impact.Core.Game.IntersectStrategy;

namespace Space_Impact.Core.Game.Player
{
	public class Hero : AbstractPlayer, IAngle, IClickable, IAffectedByBombExplosion
	{
		//Constants
		public const int HERO_SPEED = 8;

		//Hero's thrust, class definion
		public class MovementThrust : AbstractPartActor
		{
			//State
			//public bool State
			//{
			//	get; set;
			//}

			//Ower of the Thrust
			Hero Player;

			//Variables
			bool blinkState = false;
			int blinkCounter = 0;
			int blinkPeriod = 3;

			public MovementThrust(Hero player) : base(player.Name + "'s Thrust")
			{
				Animation = TextureSetLoader.SHIP1_THRUST;
				Player = player;
				RegisterPartIn(player);
			}

			protected override void DrawModification(ref ICanvasImage bitmap, CanvasDrawingSession draw)
			{
				//If there is a movement backwards, no movement actually happens
				if (Player.Direction.Vertical == SpaceDirection.VerticalDirection.DOWN && Player.Direction.Horizontal == SpaceDirection.HorizontalDirection.NONE)
				{
					bitmap = null;
					return;
				}

				//If there is no relative movement, or a weak backwards movement with a sideways movement, thrust will blink
				//(it is always implicitly moving forward... this may be wrong in a boss battle in the case of a static background)
				if
					(
					Player.Direction == SpaceDirection.None
					||
					Player.Direction.Vertical == SpaceDirection.VerticalDirection.DOWN && Player.Direction.Horizontal != SpaceDirection.HorizontalDirection.NONE
					)
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

		public bool Clicked
		{
			get
			{
				return Clickable.Clicked;
			}
		}

		public Hero() : base("Hero")
		{
			Animation = TextureSetLoader.SHIP1_BASE;
			Speed = HERO_SPEED;
			ShootingInterval = 50;
			RotationStrategy = new Rotation(this, 10, 65);

			//Hero uses our implementation of clickable event receiver
			Clickable = new ClickableImpl(IntersectsOn);

			//Hero has his thrust object
			this.thrust = new MovementThrust(this);

			//He is composed of the thrust
			ActorComposition.AddLast(this.thrust);

			//TODO fix this, ellipse collision
			/*IntersectStrategy = new DelegateIntersect
			(
				this
				, (x, y) =>
				{
					float XRadius = (float)Width / 2;
					float YRadius = (float)Height / 2;
					float ellipsePosition = Utility.square(X - x - XRadius) / Utility.square(XRadius) +
											Utility.square(Y - y - YRadius) / Utility.square(YRadius);
					if(ellipsePosition<1)Log.d(this, ellipsePosition.ToString());
					return ellipsePosition < 1;
				}
			);*/
		}

		//Fired when player is shooting, limited by the ShootingInterval property.
		//return: true if the shot was successful
		public override bool Shot()
		{
			//var up = SpaceDirection.get(SpaceDirection.VerticalDirection.UP)
			HeroBullet bullet = new HeroBullet(this, Angle);
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
			if (temporary_log_counter < 200)
			{
				temporary_log_counter++;
			}
			else
			{
				Health--; //debug todo remove
				Log.i(this, "X=" + X + " Y=" + Y +
					" HP=" + Health + "/" + MaxHealth +
					" Angle=" + Angle +
					" Horizontal=" +
					(Direction.Horizontal == SpaceDirection.HorizontalDirection.LEFT ? "Left" : (Direction.Horizontal == SpaceDirection.HorizontalDirection.RIGHT ? "Right" : "None")) +
					" Vertical=" +
					(Direction.Vertical == SpaceDirection.VerticalDirection.UP ? "Up" : (Direction.Vertical == SpaceDirection.VerticalDirection.DOWN ? "Down" : "None")));
				temporary_log_counter = 0;
			}

			//Changes speed based on strong will of the user, he can click on the hero to give him a morale boost
			if (Clicked)
			{
				Speed = 2 * HERO_SPEED;
			}
			else
			{
				Speed = HERO_SPEED;
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
			//Log.d(this,"Click "+x+" "+y+" "+ IntersectsOn(x, y));
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

		public void OnBombExplosion(IBomb bomb)
		{
			Health -= bomb.Damage;
		}

		public override void OnDeath()
		{
			Field.GameOver();
		}
	}
}
