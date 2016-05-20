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
	/// Periodically rotates Actor towards his direction within a set of dimensions
	/// </summary>
	public class Rotation : IActStrategy, IDrawModificationStrategy
	{
		public float CurrentAngleDegrees
		{
			get
			{
				//Return only half (*45) because we only hold half the value in the CurrentAngle
				return CurrentAngle * ((AngleDeltaCount / DeltaAngle) / MaxAngleDegrees) * 45;
			}
		}

		//Owner of the Strategy, his direction gets evaluated when deciding on an angle direction
		IActor Owner;

		//Angle at which the hero is rotated
		float CurrentAngle = 0;
		//Difference between each angle update
		float DeltaAngle;
		//Maximum rotation angle in absolute value
		float MaxAngle;

		//Storing the angle dimensions for backwards angle calculation purposes
		float MaxAngleDegrees;
		float AngleDeltaCount;

		public Rotation(IActor owner, int angleDeltaCount, int maxAngleDegrees)
		{
			Owner = owner;
			MaxAngleDegrees = maxAngleDegrees;
			AngleDeltaCount = angleDeltaCount;

			//MaxAngle is defined by used StraightenEffect class, which is called twice
			MaxAngle = ((float)Math.PI / 4) / ((float)90/maxAngleDegrees);

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

		bool IsMovingDown()
		{
			return Owner.Direction.Vertical == SpaceDirection.VerticalDirection.DOWN;
		}

		void ChangeAngleTowards(float angle)
		{
			if (CurrentAngle < angle)
			{
				//Increase value up to the needed value, but not after that
				if (CurrentAngle + DeltaAngle > angle)
				{
					CurrentAngle = angle;
				}
				else
				{
					CurrentAngle += DeltaAngle;
				}
			}
			else if(CurrentAngle> angle)
			{
				//Decrease value down to the needed value, but not after that
				if (CurrentAngle - DeltaAngle < angle)
				{
					CurrentAngle = angle;
				}
				else
				{
					CurrentAngle -= DeltaAngle;
				}
			}
		}

		public void Act()
		{
			//Changes the Angle to correspond to a chosen direction
			if (IsRotatingLeft())
			{
				//If moving down, we only allow half of the max angle
				if(IsMovingDown())
				{
					ChangeAngleTowards(-MaxAngle / 2);
				}
				else
				{
					ChangeAngleTowards(-MaxAngle);
				}
				
			}
			else if (IsRotatingRight())
			{
				//If moving down, we only allow half of the max angle
				if (IsMovingDown())
				{
					ChangeAngleTowards(MaxAngle / 2);
				}
				else
				{
					ChangeAngleTowards(MaxAngle);
				}
			}
			else
			{
				ChangeAngleTowards(0);
			}

			//Update the angle of the actor, but only if he is compatible
			if(Owner is IAngle)
			{
				((IAngle)Owner).Angle = CurrentAngleDegrees;
			}
		}

		public void DrawModification(ref ICanvasImage bitmap, CanvasDrawingSession draw)
		{
			//Uses effect twice to allow for up to 90 degree rotation
			StraightenEffect firstEffect = new StraightenEffect();
			firstEffect.Source = bitmap;
			firstEffect.Angle = CurrentAngle;
			StraightenEffect secondEffect = new StraightenEffect();
			secondEffect.Source = firstEffect;
			secondEffect.Angle = CurrentAngle;
			bitmap = secondEffect;
		}
	}
}
