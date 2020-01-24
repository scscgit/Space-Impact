using Space_Impact.Core.Game.ActorStrategy;
using Space_Impact.Core.Game.ActorStrategy.Rotation;
using Space_Impact.Core.Game.Character.Enemy.Bomb;
using Space_Impact.Core.Game.IntersectStrategy;
using Space_Impact.Core.Game.Object.Collectable;
using Space_Impact.Core.Game.Object.Projectile.Bullet;
using Space_Impact.Core.Game.PartActor.Thrust;
using Space_Impact.Core.Game.Player;
using Space_Impact.Core.Game.Weapon;
using Space_Impact.Graphics;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Character.Enemy
{
	/// <summary>
	/// Darkspring The Twisted.
	/// The first (and last?) boss.
	/// </summary>
	public class DarkspringTheTwisted : AbstractEnemy
	{
		//Custom thrust, class definition
		public class Thrust : AbstractMovementThrust
		{
			public const int BLINK_PERIOD = 5;

			public Thrust(IActor player) : base(player, SpaceDirection.VerticalDirection.DOWN, BLINK_PERIOD)
			{
				Animation = TextureSetLoader.SHIP12_THRUST;
			}
		}

		public DarkspringTheTwisted() : base("Darkspring The Twisted", score: 500)
		{
			Animation = TextureSetLoader.SHIP12_BASE;
			Direction = SpaceDirection.None + SpaceDirection.VerticalDirection.DOWN;

			MaxHealth = 600;
			Health = MaxHealth;

			IntersectStrategy = new SquareIntersect(this)
			{
				Percent = 65
			};

			//Approaches slowly
			Speed = 1.5f;

			Weapon = new MultiProjectileShooter
			(
				multiShot: 1
				, dispersion: 0
				, newProjectileCallback: (character, position, angle) =>
				new FireBullet<IPlayer>(character, position, angle, speed: 2, damage: 12)
			);
		}

		public override void AddedToFieldHook()
		{
			base.AddedToFieldHook();

			//Is always shooting downwards
			AddStrategy(new Shooting
			(
				owner: this
				, shootingInterval: 10
				, weaponCallback: () => Weapon
				, bulletFocusPosition: () => BulletFocusPosition
				, angleCallback: () => Utility.RandomBetween(160, 200)
			)
			{
				IsShooting = true
			});

			//Immediately rotates towards the vertical orientation direction
			AddStrategy(new TargetAngleRotation
			(
				owner: this
				, verticalOrientation: Direction.Vertical
				, angleDeltaCount: 1
				, maxAngleDegrees: 0
			)
			{
				TargetAngleRadians = AbstractRotation.DegreesToRadians(0)
			});

			AddStrategy(new EveryNActs
			(
				callback: () =>
				{
					var bomb = new Doomday(Field.Player);
					bomb.X = BulletFocusPosition.X + Utility.RandomBetween(-1, 1) * (float)bomb.Width;
					bomb.Y = BulletFocusPosition.Y;
					bomb.Speed = 0.4f * Utility.RandomBetween(1, 5);
					Field.AddActor(bomb);
				}
				, interval: 80
			));

			//Custom Thrust
			new Thrust(this);
		}

		public override void Act()
		{
			base.Act();

			//Stops when fully visible
			if (Y > Height / 5)
			{
				Speed = 0;
			}
		}

		public override void OnDeath()
		{
			base.OnDeath();

			//If the player hasn't lost, he wins
			if (Field.GameRunning)
			{
				Field.MessageBroadcastText = "Congratz, you've won!\nThe " + Name + "\nis no longer to be feared.";
				Field.GameOver();
			}
		}
	}
}
