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
	public abstract class AbstractRotation : IDrawModificationStrategy
	{
		public float CurrentAngleDegrees
		{
			get
			{
				//Relative radians to degrees conversion
				float degrees = RadiansToDegrees(Utility.NormalizeRadianAngle(CurrentRelativeAngleRadians));
				if (VerticalOrientation == SpaceDirection.VerticalDirection.DOWN)
				{
					degrees = 180 - degrees;
				}
				return degrees;
			}
		}

		//Owner of the Strategy, his direction gets evaluated when deciding on an angle direction
		protected IActor Owner;

		//Angle at which the character is rotated relative to his orientation direction
		protected float CurrentRelativeAngleRadians = 0;
		//Difference between each angle update
		protected float DeltaAngle;
		//Maximum rotation angle in absolute value
		protected float MaxAngle;

		//Storing the angle dimensions for backwards angle calculation purposes
		protected float MaxAngleDegrees;
		protected float AngleDeltaCount;

		//Direction in which the Actor is supposed to be moving with respect to the game screen.
		protected SpaceDirection.VerticalDirection VerticalOrientation;

		/// <summary>
		/// Dynamical rotation with a fraction delta differences for better visual effect, supporting both kinds of vertical orientation types of an Actor.
		/// </summary>
		/// <param name="owner">rotated Actor, his Angle gets automatically updated if he is IAngle</param>
		/// <param name="verticalOrientation">orientation of the actor, UP by default, DOWN for inversed logic</param>
		/// <param name="angleDeltaCount">number of required acts before the rotation reaches its peak and no longer has any new effect in the same direction</param>
		/// <param name="maxAngleDegrees">maximum allowed rotation angle before further movement in the same direction has no effect</param>
		public AbstractRotation(IActor owner, SpaceDirection.VerticalDirection verticalOrientation, int angleDeltaCount, int maxAngleDegrees)
		{
			Owner = owner;
			VerticalOrientation = verticalOrientation;
			AngleDeltaCount = angleDeltaCount;
			MaxAngleDegrees = maxAngleDegrees;

			//Angle can be initialized from the Actor's current state
			if (owner is IAngle)
			{
				CurrentRelativeAngleRadians = DegreeToRadians(((IAngle)owner).Angle);
			}

			//Maximum rotation angle
			MaxAngle = DegreeToRadians(maxAngleDegrees);

			//Delta is a fraction of MaxAngle
			DeltaAngle = MaxAngle / angleDeltaCount;
		}

		protected void ChangeRelativeAngleWithoutGoingAroundTowards(float angleRadians)
		{
			if (CurrentRelativeAngleRadians < angleRadians)
			{
				//Increase value up to the needed value, but not more than that
				if (CurrentRelativeAngleRadians + DeltaAngle > angleRadians)
				{
					CurrentRelativeAngleRadians = angleRadians;
				}
				else
				{
					CurrentRelativeAngleRadians += DeltaAngle;
				}
			}
			else if (CurrentRelativeAngleRadians > angleRadians)
			{
				//Decrease value down to the needed value, but not less than that
				if (CurrentRelativeAngleRadians - DeltaAngle < angleRadians)
				{
					CurrentRelativeAngleRadians = angleRadians;
				}
				else
				{
					CurrentRelativeAngleRadians -= DeltaAngle;
				}
			}
		}

		//Version dispatcher in the meantime while there are implementation differences
		protected void ChangeRelativeAngleTowards(float angleRadians)
		{
			Alternative_ChangeRelativeAngleTowards(angleRadians);
			//Old_ChangeRelativeAngleTowards(angleRadians);
		}

		protected void Alternative_ChangeRelativeAngleTowards(float angleRadians)
		{
			angleRadians = Utility.NormalizeRadianAngle(angleRadians);

			//Angle difference between the current and expected angle
			float remainingAngle = CurrentRelativeAngleRadians - angleRadians;
			if (remainingAngle < 0)
			{
				remainingAngle += DegreeToRadians(360);
			}

			//Rotates in counter-clockwise direction
			if (remainingAngle >= DegreeToRadians(180))
			{
				//Increase value up to the needed value, but not more than that
				if (DegreeToRadians(360) - remainingAngle <= DeltaAngle)
				{
					CurrentRelativeAngleRadians = angleRadians;
				}
				else
				{
					CurrentRelativeAngleRadians = Utility.NormalizeRadianAngle(CurrentRelativeAngleRadians + DeltaAngle);
				}
			}
			//Rotates in clockwise direction
			else
			{
				//Decrease value down to the needed value, but not less than that
				if (remainingAngle <= DeltaAngle)
				{
					CurrentRelativeAngleRadians = angleRadians;
				}
				else
				{
					CurrentRelativeAngleRadians = Utility.NormalizeRadianAngle(CurrentRelativeAngleRadians - DeltaAngle);
				}
			}
		}

		//Old implementation, there are still some problems, see Alternative_ method
		[Obsolete]
		protected void Old_ChangeRelativeAngleTowards(float angleRadians)
		{
			angleRadians = Utility.NormalizeRadianAngle(angleRadians);

			//Angle difference between the current and expected angle
			float remainingAngle = CurrentRelativeAngleRadians - angleRadians;

			//Rotates in counter-clockwise direction
			if
			(
				remainingAngle >= -DegreeToRadians(180) && remainingAngle < 0
				||
				remainingAngle >= DegreeToRadians(180)
			)
			{
				//Increase value up to the needed value, but not more than that
				if (CurrentRelativeAngleRadians + DeltaAngle > angleRadians)
				{
					CurrentRelativeAngleRadians = angleRadians;
				}
				else
				{
					CurrentRelativeAngleRadians = Utility.NormalizeRadianAngle(CurrentRelativeAngleRadians + DeltaAngle);
				}
			}
			//Rotates in clockwise direction
			else if
			(
				remainingAngle > -DegreeToRadians(360) && remainingAngle < -DegreeToRadians(180)
				||
				remainingAngle > 0 && remainingAngle < DegreeToRadians(180)
			)
			{
				//Decrease value down to the needed value, but not less than that
				if (CurrentRelativeAngleRadians - DeltaAngle < angleRadians)
				{
					CurrentRelativeAngleRadians = angleRadians;
				}
				else
				{
					CurrentRelativeAngleRadians = Utility.NormalizeRadianAngle(CurrentRelativeAngleRadians - DeltaAngle);
				}
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
