﻿using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.ActorStrategy.Rotation
{
	/// <summary>
	/// Rotates towards the target Actor.
	/// </summary>
	public class TargetActorAngleRotation : TargetAngleRotation
	{
		protected IActor Target;

		public TargetActorAngleRotation(IActor owner, IActor target, SpaceDirection.VerticalDirection verticalOrientation, int angleDeltaCount, int maxAngleDegrees)
			: base(owner, verticalOrientation, angleDeltaCount, maxAngleDegrees)
		{
			Target = target;
		}

		public override void Act()
		{
			double deltaX = Target.X - Owner.X;
			double deltaY = Target.Y - Owner.Y;

			//Division by zero could destroy the universe as we know it
			float atan = (deltaX == 0 ? 0 : (float)Math.Atan(deltaY / deltaX));

			//Angles are calculated from the 90 degree start point based on the texture, reversed vertically if the direction is DOWN
			if (VerticalOrientation == SpaceDirection.VerticalDirection.DOWN)
			{
				atan = DegreeToRadians(90) - atan;
			}
			else
			{
				atan = atan + DegreeToRadians(90);
			}

			//In the 2nd and 3rd quadrant add 180 degrees
			if (deltaX < 0)
			{
				atan = atan + DegreeToRadians(180);
			}

			//In the 1st quadrant add 360 degrees if (deltaX >= 0 && deltaY < 0) implicitly by normalization 
			TargetAngleRadians = Utility.NormalizeRadianAngle(atan);

			//Implicit Act
			base.Act();
		}
	}
}
