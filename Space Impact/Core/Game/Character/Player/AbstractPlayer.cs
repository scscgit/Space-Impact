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
	public abstract class AbstractPlayer : AbstractCharacter, IPlayer
	{
		protected AbstractPlayer(string name) : base(name)
		{
			//Initialization of the default Shooting parameters
			Shooting = false;
			ShootingInterval = 10;
			ShootingCooldown = 0;
		}

		//State of shooting (controlled by the user)
		public bool Shooting
		{
			get; set;
		}

		//Interval between individual fired shots in game cycles
		public int ShootingInterval
		{
			get; set;
		}

		//Remaining cooldown before next fired shot, 0 if ready to shoot
		public int ShootingCooldown
		{
			get; protected set;
		}

		//Getter that a Bullet uses to see where it should be created
		public virtual Position BulletFocusPosition
		{
			get
			{
				Position position = new Position();
				//Moves to the player's center coordinates
				position.X = X + (float)Width / 2;
				position.Y = Y + (float)Height / 2;
				return position;
			}
		}

		public abstract bool Shot();

		public override void Act()
		{
			base.Act();

			//TODO: fix reset of cooldown when interval gets changed during cooldown, idk, hotswap weapon custom cooldowns?
			//Process shooting cooldown
			if (ShootingCooldown > 0)
			{
				//Decrements cooldown
				ShootingCooldown--;
			}
			//If the Player is shooting, 
			else if (Shooting && ShootingCooldown == 0)
			{
				//Creates a new bullet
				Shot();
				Log.i(this, "Shot() call has finished");

				//Resets cooldown
				ShootingCooldown = ShootingInterval;
			}
		}
	}
}
