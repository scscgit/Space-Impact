using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Space_Impact.Support;

namespace Space_Impact.Core.Game.ActorStrategy
{
	/// <summary>
	/// Periodically rotates Actor towards his direction within a set of dimensions.
	/// </summary>
	public class FlyingRotation : IActStrategy, IDrawModificationStrategy
	{
		public float CurrentAngleDegrees
		{
			get
			{
				//Relative radians to degrees conversion
				float degrees = RadiansToDegrees(CurrentRelativeAngleRadians);
				if (VerticalOrientation == SpaceDirection.VerticalDirection.DOWN)
				{
					degrees = 180 - degrees;
				}
				return Utility.NormalizeDegreeAngle(degrees);
			}
		}

		//Owner of the Strategy, his direction gets evaluated when deciding on an angle direction
		IActor Owner;

		//Angle at which the character is rotated relative to his orientation direction
		float CurrentRelativeAngleRadians = 0;
		//Difference between each angle update
		float DeltaAngle;
		//Maximum rotation angle in absolute value
		float MaxAngle;

		//Storing the angle dimensions for backwards angle calculation purposes
		float MaxAngleDegrees;
		float AngleDeltaCount;

		//Direction in which the Actor is supposed to be moving with respect to the game screen.
		SpaceDirection.VerticalDirection VerticalOrientation;

		/// <summary>
		/// Dynamical rotation with a fraction delta differences for better visual effect, supporting both kinds of vertical orientation types of an Actor.
		/// </summary>
		/// <param name="owner">rotated Actor, his Angle gets automatically updated if he is IAngle</param>
		/// <param name="verticalOrientation">orientation of the actor, UP by default, DOWN for inversed logic</param>
		/// <param name="angleDeltaCount">number of required acts before the rotation reaches its peak and no longer has any new effect in the same direction</param>
		/// <param name="maxAngleDegrees">maximum allowed rotation angle before further movement in the same direction has no effect</param>
		public FlyingRotation(IActor owner, SpaceDirection.VerticalDirection verticalOrientation, int angleDeltaCount, int maxAngleDegrees)
		{
			Owner = owner;
			VerticalOrientation = verticalOrientation;
			AngleDeltaCount = angleDeltaCount;
			MaxAngleDegrees = maxAngleDegrees;

			//Maximum rotation angle
			MaxAngle = DegreeToRadians(maxAngleDegrees);

			//Delta is a fraction of MaxAngle
			DeltaAngle = MaxAngle / angleDeltaCount;
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

		void ChangeRelativeAngleTowards(float angle)
		{
			if (CurrentRelativeAngleRadians < angle)
			{
				//Increase value up to the needed value, but not more than that
				if (CurrentRelativeAngleRadians + DeltaAngle > angle)
				{
					CurrentRelativeAngleRadians = angle;
				}
				else
				{
					CurrentRelativeAngleRadians += DeltaAngle;
				}
			}
			else if (CurrentRelativeAngleRadians > angle)
			{
				//Decrease value down to the needed value, but not less than that
				if (CurrentRelativeAngleRadians - DeltaAngle < angle)
				{
					CurrentRelativeAngleRadians = angle;
				}
				else
				{
					CurrentRelativeAngleRadians -= DeltaAngle;
				}
			}
		}

		public void Act()
		{
			//Changes the Angle to correspond to a chosen direction
			if (IsRotatingLeft())
			{
				//If moving down, we only allow half of the max angle
				if (IsMovingBack())
				{
					ChangeRelativeAngleTowards(-MaxAngle / 2);
				}
				else
				{
					ChangeRelativeAngleTowards(-MaxAngle);
				}

			}
			else if (IsRotatingRight())
			{
				//If moving down, we only allow half of the max angle
				if (IsMovingBack())
				{
					ChangeRelativeAngleTowards(MaxAngle / 2);
				}
				else
				{
					ChangeRelativeAngleTowards(MaxAngle);
				}
			}
			else
			{
				ChangeRelativeAngleTowards(0);
			}

			//Update the angle of the actor, but only if he is compatible
			if (Owner is IAngle)
			{
				((IAngle)Owner).Angle = CurrentAngleDegrees;
			}
		}

		public static float DegreeToRadians(float degrees)
		{
			return degrees * (float)Math.PI / 180;
		}
		public static float RadiansToDegrees(float radians)
		{
			return radians * 180 / (float)Math.PI;
		}

		//Rotates the bitmap by repeatedly applying up to 45 degree effect
		public static void DrawModification(ref ICanvasImage bitmap, CanvasDrawingSession draw, float angleRadians)
		{
			float maxStep = (float)Math.PI / 4;

			angleRadians = Utility.NormalizeRadianAngle(angleRadians);

			for (float remainingAngleRadians = angleRadians; remainingAngleRadians > 0; remainingAngleRadians -= maxStep)
			{
				StraightenEffect rotate = new StraightenEffect();
				rotate.Source = bitmap;
				if (remainingAngleRadians > maxStep)
				{
					rotate.Angle = maxStep;
				}
				else
				{
					rotate.Angle = remainingAngleRadians;
				}
				bitmap = rotate;
			}
		}

		public void DrawModification(ref ICanvasImage bitmap, CanvasDrawingSession draw)
		{
			//In the case of a downward movement, rotates the character by 180 degrees first
			if (VerticalOrientation == SpaceDirection.VerticalDirection.DOWN)
			{
				DrawModification(ref bitmap, draw, (float)Math.PI - CurrentRelativeAngleRadians);
			}
			else
			{
				DrawModification(ref bitmap, draw, CurrentRelativeAngleRadians);
			}
		}
	}
}
