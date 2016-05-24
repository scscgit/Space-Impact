using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Space_Impact.Core.Game.ActorStrategy;
using Space_Impact.Core.Game.Object.Collectable;
using Space_Impact.Core.Game.Object.Collectable.WeaponUpgrade;
using Space_Impact.Support;
using Space_Impact.Core.Game.ActorStrategy.Rotation;
using Space_Impact.Core.Game.PartActor.Thrust;

namespace Space_Impact.Core.Game.Character.Enemy
{
	public class Lakebeam : AbstractEnemy
	{
		//Custom thrust, class definition
		public class Thrust : AbstractMovementThrust
		{
			public const int BLINK_PERIOD = 5;

			public Thrust(IActor player) : base(player, SpaceDirection.VerticalDirection.DOWN, BLINK_PERIOD)
			{
				Animation = TextureSetLoader.SHIP2_THRUST;
			}
		}

		public Lakebeam() : base("Lakebeam", score: 50)
		{
			Animation = TextureSetLoader.SHIP2_BASE;
			Direction = SpaceDirection.None + SpaceDirection.VerticalDirection.DOWN;
		}

		public override void AddedToFieldHook()
		{
			base.AddedToFieldHook();

			AddStrategy(new TargetActorAngleRotation
			(
				owner: this
				, target: Field.Player
				, verticalOrientation: Direction.Vertical
				, angleDeltaCount: 100
				, maxAngleDegrees: 120
			));

			AddStrategy(new Acceleration
			(
				owner: this
				, deltaSpeed: 0.01f
				, targetSpeed: Speed * 4
			));

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
			//1:3 chance of MultiBulletShooter
			if (Utility.RandomBetween(0, 2) == 0)
			{
				return new UMultiBulletShooter();
			}
			return null;
		}
	}
}
