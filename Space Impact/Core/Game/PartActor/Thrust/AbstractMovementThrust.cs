using Microsoft.Graphics.Canvas;
using Space_Impact.Core.Game.ActorStrategy;
using Space_Impact.Core.Game.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.PartActor.Thrust
{
	public abstract class AbstractMovementThrust : AbstractPartActor
	{
		//Variables
		bool BlinkState = false;
		int BlinkCounter = 0;
		int BlinkPeriod;

		//Vertical type of direction of the thrust
		SpaceDirection.VerticalDirection VerticalOrientation;

		protected AbstractMovementThrust(IActor owner, SpaceDirection.VerticalDirection verticalOrientation, int blinkPeriod) : base(owner, "Thrust")
		{
			VerticalOrientation = verticalOrientation;
			BlinkPeriod = blinkPeriod;
		}

		/// <summary>
		/// Represents backwards movement of an Actor
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

		protected override void DrawModification(ref ICanvasImage bitmap, CanvasDrawingSession draw)
		{
			base.DrawModification(ref bitmap, draw);

			//Uses the same draw strategies as its Owner
			foreach (IDrawModificationStrategy strategy in Owner.DrawModificationStrategies)
			{
				strategy.DrawModification(ref bitmap, draw);
			}

			//If there is a movement backwards, no movement actually happens
			if (IsMovingBack() && Owner.Direction.Horizontal == SpaceDirection.HorizontalDirection.NONE)
			{
				bitmap = null;
				return;
			}

			//If there is no relative movement, or a weak backwards movement with a sideways movement, thrust will blink
			//(it is always implicitly moving forward... this may be wrong in a boss battle in the case of a static background)
			if
				(
				Owner.Direction == SpaceDirection.None
				||
				IsMovingBack() && Owner.Direction.Horizontal != SpaceDirection.HorizontalDirection.NONE
				)
			{
				if (++BlinkCounter > BlinkPeriod)
				{
					BlinkCounter = 0;
					BlinkState = !BlinkState;
					if (!BlinkState)
					{
						bitmap = null;
						return;
					}
				}
			}
		}
	}
}
