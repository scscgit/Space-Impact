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

		TargetActorAngleRotation TargetRotationStrategy;

		public Venomflare() : base("Venomflare", score: 100)
		{
			Animation = TextureSetLoader.SHIP3_BASE;
			Direction = SpaceDirection.None + SpaceDirection.VerticalDirection.DOWN;
		}

		public override void AddedToFieldHook()
		{
			base.AddedToFieldHook();

			TargetRotationStrategy = new TargetActorAngleRotation
			(
				owner: this
				, target: Field.Player
				, verticalOrientation: Direction.Vertical
				, angleDeltaCount: 30
				, maxAngleDegrees: 160
			);

			//Every N acts looks at the Player's position and rotates towards him
			AddStrategy(new EveryNActs
			(
				callback: () =>
				{
					TargetRotationStrategy.TargetAngleRadians = TargetActorAngleRotation.AngleBetweenActorsRadians(this, Field.Player, true);
				}
				, interval: 100
			));

			AddStrategy(TargetRotationStrategy);

			//Custom Thrust
			new Thrust(this);
		}

		protected override ICollectable DropLoot()
		{
			//1:6 chance of MultiBulletShooter
			if (Utility.RandomBetween(0, 5) == 0)
			{
				return new UMultiBulletShooter();
			}
			return null;
		}
	}
}
