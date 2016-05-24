using Space_Impact.Core.Game.ActorStrategy;
using Space_Impact.Core.Game.ActorStrategy.Rotation;
using Space_Impact.Core.Game.Object.Collectable;
using Space_Impact.Core.Game.Object.Collectable.WeaponUpgrade;
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
	public class Venomflare : AbstractEnemy
	{
		//Custom thrust, class definition
		public class Thrust : AbstractMovementThrust
		{
			public const int BLINK_PERIOD = 5;

			public Thrust(IActor player) : base(player, SpaceDirection.VerticalDirection.DOWN, BLINK_PERIOD)
			{
				Animation = TextureSetLoader.SHIP3_THRUST;
			}
		}

		TargetAngleRotation TargetRotationStrategy;

		public Venomflare() : base("Venomflare", score: 100)
		{
			Animation = TextureSetLoader.SHIP3_BASE;
			Direction = SpaceDirection.None + SpaceDirection.VerticalDirection.DOWN;
		}

		public override void AddedToFieldHook()
		{
			base.AddedToFieldHook();

			//Strategy that does not automatically rotate but instead, needs to be rotated manually
			TargetRotationStrategy = new TargetAngleRotation
			(
				owner: this
				, verticalOrientation: Direction.Vertical
				, angleDeltaCount: 45
				, maxAngleDegrees: 160
			);

			//Every N acts looks at the Player's position and rotates towards approximately towards him
			AddStrategy(new EveryNActs
			(
				callback: () =>
				{
					float angle = TargetActorAngleRotation.AngleBetweenActorsRadians(this, Field.Player, true);

					//Some additional variance
					angle += AbstractRotation.DegreesToRadians(Utility.RandomBetween(-30, 30));

					TargetRotationStrategy.TargetAngleRadians = angle;
				}
				, interval: 40
			));

			AddStrategy(TargetRotationStrategy);

			//Custom Thrust
			new Thrust(this);

			//Starts moving in the opposite direction than where he spawned from
			if (X < Field.Size.Width / 2)
			{
				Direction += SpaceDirection.HorizontalDirection.RIGHT;
			}
			else
			{
				Direction += SpaceDirection.HorizontalDirection.LEFT;
			}
		}

		protected override ICollectable DropLoot()
		{
			//1:6 chance of MultiBulletShooter
			if (Utility.RandomBetween(0, 5) == 0)
			{
				return new UMultiBulletShooter();
			}
			//Additional 1:3 chance of PowerUp
			else if (Utility.RandomBetween(0, 2) == 0)
			{
				return new PowerUp();
			}
			return null;
		}
	}
}
