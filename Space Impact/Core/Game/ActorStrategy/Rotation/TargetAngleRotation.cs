using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.ActorStrategy.Rotation
{
	public class TargetAngleRotation : AbstractRotation, IActStrategy
	{
		/// <summary>
		/// Angle to which the Actor is being rotated.
		/// </summary>
		public float TargetAngleRadians
		{
			get; set;
		} = 0;

		public TargetAngleRotation(IActor owner, SpaceDirection.VerticalDirection verticalOrientation, int angleDeltaCount, int maxAngleDegrees)
			: base(owner, verticalOrientation, angleDeltaCount, maxAngleDegrees)
		{
		}

		public virtual void Act()
		{
			//TODO implement limitation of angle rotation for limited actors (whose MaxAngle is less than max)
			//if (TargetAngleRadians < -MaxAngle)
			//{
			//	TargetAngleRadians = -MaxAngle;
			//}
			//else if (TargetAngleRadians > MaxAngle)
			//{
			//	TargetAngleRadians = MaxAngle;
			//}

			ChangeRelativeAngleTowards(TargetAngleRadians);

			//Update the angle of the actor, but only if he is compatible
			if (Owner is IAngle)
			{
				((IAngle)Owner).Angle = CurrentAngleDegrees;
			}
		}
	}
}
