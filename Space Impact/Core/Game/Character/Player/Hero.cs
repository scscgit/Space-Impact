﻿using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Space_Impact.Support;
using Space_Impact.Core.Game.PartActor.Thrust;
using Space_Impact.Core.Game.Character.Enemy.Bomb;
using Space_Impact.Core.Game.IntersectStrategy;
using Space_Impact.Core.Game.Weapon;
using Space_Impact.Core.Game.ActorStrategy.Rotation;
using Space_Impact.Core.Game.Object.Projectile.Bullet;

namespace Space_Impact.Core.Game.Player
{
	public class Hero : AbstractPlayer, IClickable, IAffectedByBombExplosion
	{
		//Constants
		public const int HERO_SPEED = 8;
		public const int HERO_HEALTH = 500;

		//Hero's thrust, class definition
		public class MovementThrust : AbstractMovementThrust
		{
			public const int BLINK_PERIOD = 5;

			public MovementThrust(Hero player) : base(player, SpaceDirection.VerticalDirection.UP, BLINK_PERIOD)
			{
				Animation = TextureSetLoader.SHIP1_THRUST;
			}
		}

		int temporary_log_counter = 0;

		//Access to hero's strategy, should be only used for DrawModification purposes of other classes because Act() would collide
		public FlyingDirectionRotation RotationStrategy
		{
			get; private set;
		}

		//Clickable implementation manager
		IClickable Clickable;

		/// <summary>
		/// Represents state of being clicked on by mouse, or a different kind of pointer.
		/// </summary>
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
			MaxHealth = HERO_HEALTH;
			Health = HERO_HEALTH;
			Speed = HERO_SPEED;
			ShootingStrategy.ShootingInterval = 50;

			AddStrategy(new FlyingDirectionRotation
			(
				owner: this
				, verticalOrientation: SpaceDirection.VerticalDirection.UP
				, angleDeltaCount: 35
				, maxAngleDegrees: 75
			));

			//Default weapon
			Weapon = new MultiProjectileShooter
			(
				multiShot: 1
				, dispersion: 20
				, newProjectileCallback: (character, position, angle) =>
				new HeroBullet(character, position, angle)
			);

			//He is composed of the thrust
			new MovementThrust(this);

			//Custom square intersect percentage
			IntersectStrategy = new SquareIntersect(this)
			{
				Percent = 60
			};

			//Hero uses our implementation of clickable event receiver
			Clickable = new ClickableImpl(IntersectsOn);

			//TODO fix this and implement ellipse intersect collision instead of default square
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

		public override void Act()
		{
			base.Act();

			//Periodical logging of a Hero state
			if (temporary_log_counter < 200)
			{
				temporary_log_counter++;
			}
			else
			{
				Log.i(this, "X=" + X + " Y=" + Y +
					" HP=" + Health + "/" + MaxHealth +
					" Angle=" + Angle +
					" Horizontal=" +
					(Direction.Horizontal == SpaceDirection.HorizontalDirection.LEFT ? "Left" : (Direction.Horizontal == SpaceDirection.HorizontalDirection.RIGHT ? "Right" : "None")) +
					" Vertical=" +
					(Direction.Vertical == SpaceDirection.VerticalDirection.UP ? "Up" : (Direction.Vertical == SpaceDirection.VerticalDirection.DOWN ? "Down" : "None")));
				temporary_log_counter = 0;
			}

			//Act operation of the Clickable implementation
			Clickable.Act();

			//Changes speed based on strong will of the user, he can click on the hero to give him a morale boost
			if (Clicked)
			{
				Speed = 1.5f * HERO_SPEED;
			}
			else
			{
				Speed = HERO_SPEED;
			}
		}

		public override void AddedToFieldHook()
		{
			base.AddedToFieldHook();
			//Field.AddActor(this.thrust);
		}
		protected override void DrawModification(ref ICanvasImage bitmap, CanvasDrawingSession draw)
		{
			base.DrawModification(ref bitmap, draw);

			//Rotates the player in the direction where he is moving
			//(this is implemented automatically within a list in the AbstractActor)
			//RotationStrategy.DrawModification(ref bitmap, draw);
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

		public void OnBombExplosion(IBomb bomb)
		{
			Health -= bomb.Damage;
		}
	}
}
