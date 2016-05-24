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
using Space_Impact.Core.Game.Weapon;
using Space_Impact.Core.Game.Object.Projectile.Bullet;
using Space_Impact.Core.Game.Player;

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

			Weapon = new MultiProjectileShooter
			(
				multiShot: 0
				, dispersion: 0
				, newProjectileCallback: (character, position, angle) =>
				new FireBullet<IPlayer>(character, position, angle, speed: 5, damage: 5)
			);
		}

		public override void AddedToFieldHook()
		{
			base.AddedToFieldHook();

			TargetActorAngleRotation rotationStrategy = new TargetActorAngleRotation
			(
				owner: this
				, target: Field.Player
				, verticalOrientation: Direction.Vertical
				, angleDeltaCount: 100
				, maxAngleDegrees: 120
			);
			AddStrategy(rotationStrategy);

			//Initialization of the default Shooting parameters via strategy
			AddStrategy(new Shooting
			(
				owner: this
				, shootingInterval: 20
				, weaponCallback: () => Weapon
				, bulletFocusPosition: () => BulletFocusPosition
				, angleCallback: () => rotationStrategy.CurrentAngleDegrees
			)
			{
				IsShooting = true
			});

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
