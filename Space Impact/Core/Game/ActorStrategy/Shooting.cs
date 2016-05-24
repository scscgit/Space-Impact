using Space_Impact.Core.Game.Character;
using Space_Impact.Core.Game.Weapon;
using Space_Impact.Graphics;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.ActorStrategy
{
	public delegate IWeapon WeaponCallback();
	public delegate Position BulletFocusPositionCallback();
	public delegate float AngleCallback();

	/// <summary>
	/// Strategy for Shooting.
	/// </summary>
	public class Shooting : IActStrategy
	{
		WeaponCallback WeaponCallback;
		BulletFocusPositionCallback BulletFocusPositionCallback;
		AngleCallback AngleCallback;

		ICharacter Owner;

		//State of shooting (controlled by the user)
		public bool IsShooting
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

		public Shooting(ICharacter owner, int shootingInterval, WeaponCallback weaponCallback, BulletFocusPositionCallback bulletFocusPosition, AngleCallback angleCallback)
		{
			//Parameter initialization
			Owner = owner;
			IsShooting = false;
			ShootingInterval = shootingInterval;
			ShootingCooldown = 0;

			//Storing callbacks
			WeaponCallback = weaponCallback;
			BulletFocusPositionCallback = bulletFocusPosition;
			AngleCallback = angleCallback;
		}

		protected bool Shot()
		{
			IWeapon weapon = WeaponCallback();
			if (weapon != null)
			{
				return weapon.Shot(Owner, BulletFocusPositionCallback(), AngleCallback());
			}
			return false;
		}

		public void Act()
		{
			//TODO: fix reset of cooldown when interval gets changed during cooldown, idk, hotswap weapon custom cooldowns?
			//Process shooting cooldown
			if (ShootingCooldown > 0)
			{
				//Decrements cooldown
				ShootingCooldown--;
			}
			//If the Player is shooting, 
			else if (IsShooting && ShootingCooldown == 0)
			{
				//Creates a new projectile
				//Fired when player is shooting, limited by the ShootingInterval property.
				bool shotResult = Shot();
				Log.i(this, "Shot() call has finished, result is " + (shotResult ? "true" : "false"));

				//Resets cooldown
				ShootingCooldown = ShootingInterval;
			}
		}
	}
}
