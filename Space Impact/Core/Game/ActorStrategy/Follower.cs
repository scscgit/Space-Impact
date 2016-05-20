using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.ActorStrategy
{
	/// <summary>
	/// Strategy of following the target.
	/// </summary>
	public class Follower : IActStrategy
	{
		IActor Owner;
		IActor Target;

		//Bounds defining within what direction from target will the following stop
		public int WidthBounds { get; set; }
		public int HeightBounds { get; set; }

		public Follower(IActor owner, IActor target)
		{
			Owner = owner;
			Target = target;
		}

		//Direction towards the border of a target, may have difference between intersection on opposite sides
		[Obsolete]
		SpaceDirection.HorizontalDirection HorizontalDirectionToTargetBorder()
		{
			if (Target.X + Target.Width < Owner.X + Owner.Width)
			{
				return SpaceDirection.HorizontalDirection.LEFT;
			}
			else if (Target.X > Owner.X)
			{
				return SpaceDirection.HorizontalDirection.RIGHT;
			}
			else
			{
				return SpaceDirection.HorizontalDirection.NONE;
			}
		}
		[Obsolete]
		SpaceDirection.VerticalDirection VerticalDirectionToTargeBordert()
		{
			if (Target.Y + Target.Height < Owner.Y + Owner.Height)
			{
				return SpaceDirection.VerticalDirection.UP;
			}
			else if (Target.Y > Owner.Y)
			{
				return SpaceDirection.VerticalDirection.DOWN;
			}
			else
			{
				return SpaceDirection.VerticalDirection.NONE;
			}
		}

		SpaceDirection.HorizontalDirection HorizontalDirectionToTarget()
		{
			if (Target.X + Target.Width /2 - WidthBounds / 2 < Owner.X + Owner.Width / 2 - WidthBounds / 2)
			{
				return SpaceDirection.HorizontalDirection.LEFT;
			}
			else if (Target.X + Target.Width / 2 - WidthBounds / 2 > Owner.X + Owner.Width / 2 - WidthBounds / 2)
			{
				return SpaceDirection.HorizontalDirection.RIGHT;
			}
			else
			{
				return SpaceDirection.HorizontalDirection.NONE;
			}
		}

		SpaceDirection.VerticalDirection VerticalDirectionToTarget()
		{
			if (Target.Y + Target.Height / 2 - HeightBounds / 2 < Owner.Y + Owner.Height / 2 - HeightBounds / 2)
			{
				return SpaceDirection.VerticalDirection.UP;
			}
			else if (Target.Y + Target.Height / 2 - HeightBounds / 2 > Owner.Y + Owner.Height / 2 - HeightBounds / 2)
			{
				return SpaceDirection.VerticalDirection.DOWN;
			}
			else
			{
				return SpaceDirection.VerticalDirection.NONE;
			}
		}

		public void Act()
		{
			if (Target != null)
			{
				//Starts the movement towards the target
				Owner.Direction = SpaceDirection.get(HorizontalDirectionToTarget(), VerticalDirectionToTarget());
			}
			else
			//Target is lost from the world
			{
				Owner.Direction = SpaceDirection.None;
			}
		}
	}
}
