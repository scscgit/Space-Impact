using Space_Impact.Core.Game.ActorStrategy;
using Space_Impact.Core.Game.Character;
using Space_Impact.Graphics;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Player
{
	public abstract class AbstractPlayer : AbstractCharacter, IPlayer, IAngle
	{
		const int DEFAULT_SHOT_INTERVAL = 10;

		protected Shooting ShootingStrategy;

		/// <summary>
		/// Angle in degrees.
		/// </summary>
		public float Angle
		{
			get; set;
		}

		//State of shooting (controlled by the user)
		public bool Shooting
		{
			get
			{
				return ShootingStrategy.IsShooting;
			}
			set
			{
				ShootingStrategy.IsShooting = value;
			}
		}

		protected AbstractPlayer(string name) : base(name)
		{
			//Initialization of the default Shooting parameters via strategy
			ShootingStrategy = new Shooting
			(
				owner: this
				, shootingInterval: DEFAULT_SHOT_INTERVAL
				, weaponCallback: () => Weapon
				, bulletFocusPosition: () => BulletFocusPosition
				, angleCallback: () => Angle
			);

			AddStrategy(ShootingStrategy);
		}

		/// <summary>
		/// Event of Player dying.
		/// Field needs to be notified.
		/// </summary>
		public override void OnDeath()
		{
			Field.GameOver();
		}

		//Collisions don't work quite as well as I expected, so the currently used implementation only influences NPCs
		public override bool CollidesWith(IActor actor)
		{
			return false;
		}
	}
}
