﻿using Space_Impact.Core.Game.ActorStrategy.Rotation;
using Space_Impact.Core.Game.Object.Collectable;
using Space_Impact.Core.Game.PartActor.Thrust;
using Space_Impact.Graphics;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Character.Enemy
{
	public class Waveghost : AbstractEnemy, IAngle
	{
		//Custom thrust, class definition
		public class Thrust : AbstractMovementThrust
		{
			public const int BLINK_PERIOD = 20;

			public Thrust(IActor player) : base(player, SpaceDirection.VerticalDirection.UP, BLINK_PERIOD)
			{
				Animation = TextureSetLoader.SHIP4_THRUST;
			}
		}

		//Representative values used as a rotation direction
		const int CLOCKWISE = -1;
		const int COUNTERCLOCKWISE = 1;

		//Constants
		const float WAVEGHOST_SPEED = 5;

		Thrust WaveghostThrust;

		//Waveghost controls Speed and gives it additional purpose
		float RealSpeed;

		TargetAngleRotation TargetRotationStrategy;

		//Rotational direction
		int RotationDirection;

		public float Angle
		{
			get; set;
		}

		public Waveghost() : base("Waveghost", score: 150)
		{
			Animation = TextureSetLoader.SHIP4_BASE;
			Direction = SpaceDirection.None + SpaceDirection.VerticalDirection.DOWN;
			RealSpeed = WAVEGHOST_SPEED;

			//Random rotation direction
			RotationDirection = Utility.RandomBetween(0, 1) == 0 ? CLOCKWISE : COUNTERCLOCKWISE;
		}

		public override void AddedToFieldHook()
		{
			base.AddedToFieldHook();

			//Strategy that does not automatically rotate but instead, needs to be rotated manually
			TargetRotationStrategy = new TargetAngleRotation
			(
				owner: this
				, verticalOrientation: Direction.Vertical
				, angleDeltaCount: 200
				, maxAngleDegrees: 360
			);

			AddStrategy(TargetRotationStrategy);

			//Custom Thrust
			WaveghostThrust = new Thrust(this);
		}

		protected override ICollectable DropLoot()
		{
			return null;
		}

		public override void Act()
		{
			Direction = SpaceDirection.GetFromAngle(Angle);

			//Always rotates
			TargetRotationStrategy.TargetAngleRadians = TargetRotationStrategy.CurrentRelativeAngleRadians + RotationDirection;
			//Doesn't work from degrees, but why???:
			//TargetRotationStrategy.TargetAngleRadians = AbstractRotation.DegreeToRadians(Angle + RotationDirection);

			//Initializing Speed
			Speed = RealSpeed;

			//Does not work as expected, but after a bit of tweaking, could be used to do small bursts of speed
			//if (WaveghostThrust.IsTurnedOn)
			//{
			//	Speed = RealSpeed / 3;
			//}
			//else
			//{
			//	Speed = RealSpeed;
			//}

			//Reducing speed if the direction is towards the outside of the Game Field
			//Reduction of X and Y stacks towards the final Speed
			if
			(
				Direction.Horizontal.Equals(SpaceDirection.HorizontalDirection.LEFT)
				&&
				X < Field.Size.Width / 4
				||
				Direction.Horizontal.Equals(SpaceDirection.HorizontalDirection.RIGHT)
				&&
				X > 3 * Field.Size.Width / 4
			)
			{
				Speed /= 2;
			}
			if
			(
				Direction.Vertical.Equals(SpaceDirection.VerticalDirection.UP)
				&&
				Y < Field.Size.Height / 4
				||
				Direction.Vertical.Equals(SpaceDirection.VerticalDirection.DOWN)
				&&
				Y > 3 * Field.Size.Height / 4
			)
			{
				Speed /= 2;
			}

			//Implicit Act, uses the real Speed for rest of calculations
			base.Act();
		}
	}
}