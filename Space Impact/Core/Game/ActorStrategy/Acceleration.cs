using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.ActorStrategy
{
	/// <summary>
	/// Strategy of linearly accelerating the Speed of an Actor.
	/// </summary>
	public class Acceleration : IActStrategy
	{
		IActor Owner;

		float StartSpeed;
		float DeltaSpeed;
		float TargetSpeed;

		/// <summary>
		/// Linear acceleration.
		/// </summary>
		/// <param name="owner">Actor that should be accelerated.</param>
		/// <param name="deltaSpeed">Speed difference to be increased in each Act.</param>
		/// <param name="targetSpeed">Maximum obtainable speed.</param>
		public Acceleration(IActor owner, float deltaSpeed, float targetSpeed)
		{
			Owner = owner;
			StartSpeed = Owner.Speed;
			DeltaSpeed = deltaSpeed;
			TargetSpeed = targetSpeed;
		}

		public void Act()
		{
			float currentSpeed = Owner.Speed;
			if (currentSpeed + DeltaSpeed > TargetSpeed)
			{
				currentSpeed = TargetSpeed;
			}
			else
			{
				currentSpeed += DeltaSpeed;
			}

			//Updating the owner's speed
			Owner.Speed = currentSpeed;
		}
	}
}
