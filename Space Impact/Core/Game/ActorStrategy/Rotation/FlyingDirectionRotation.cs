using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Space_Impact.Support;

namespace Space_Impact.Core.Game.ActorStrategy.Rotation
{
	/// <summary>
	/// Periodically rotates Actor towards his direction within a set of dimensions.
	/// </summary>
	public class FlyingDirectionRotation : TargetAngleRotation, IActStrategy
	{
		public FlyingDirectionRotation(IActor owner, SpaceDirection.VerticalDirection verticalOrientation, int angleDeltaCount, int maxAngleDegrees)
			: base(owner, verticalOrientation, angleDeltaCount, maxAngleDegrees)
		{
		}

		bool IsRotatingLeft()
		{
			return Owner.Direction.Horizontal == SpaceDirection.HorizontalDirection.LEFT;
		}

		bool IsRotatingRight()
		{
			return Owner.Direction.Horizontal == SpaceDirection.HorizontalDirection.RIGHT;
		}

		/// <summary>
		/// Represents backwards movement of an Actor, used for limiting his sideways movement by the half.
		/// </summary>
		/// <returns>true if the Actor is moving backwards</returns>
		bool IsMovingBack()
		{
			if (VerticalOrientation == SpaceDirection.VerticalDirection.UP)
			{
				return Owner.Direction.Vertical == SpaceDirection.VerticalDirection.DOWN;
			}
			else if (VerticalOrientation == SpaceDirection.VerticalDirection.DOWN)
			{
				return Owner.Direction.Vertical == SpaceDirection.VerticalDirection.UP;
			}
			else
			{
				return false;
			}
		}

		override public void Act()
		{
			//Changes the Angle to correspond to a chosen direction
			if (IsRotatingLeft())
			{
				//If moving down, we only allow half of the max angle
				if (IsMovingBack())
				{
					TargetAngleRadians = -MaxAngle / 2;
				}
				else
				{
					TargetAngleRadians = -MaxAngle;
				}

			}
			else if (IsRotatingRight())
			{
				//If moving down, we only allow half of the max angle
				if (IsMovingBack())
				{
					TargetAngleRadians = MaxAngle / 2;
				}
				else
				{
					TargetAngleRadians = MaxAngle;
				}
			}
			else
			{
				TargetAngleRadians = 0;
			}

			//Implicitly rotates towards the Target Angle
			base.Act();
		}
	}
}
