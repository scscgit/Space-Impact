using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game
{
	/// <summary>
	/// 2D direction representation, now featuring special operators with a custom Algebraic system.
	/// Created by Steve on 29.2.2016 for Java, heavily modified since.
	/// </summary>
	public class SpaceDirection
	{
		public enum HorizontalDirection
		{
			LEFT, NONE, RIGHT
		}

		public enum VerticalDirection
		{
			UP, NONE, DOWN
		}

		static readonly SpaceDirection LEFT_UP = new SpaceDirection(HorizontalDirection.LEFT, VerticalDirection.UP);
		static readonly SpaceDirection NONE_UP = new SpaceDirection(HorizontalDirection.NONE, VerticalDirection.UP);
		static readonly SpaceDirection RIGHT_UP = new SpaceDirection(HorizontalDirection.RIGHT, VerticalDirection.UP);
		static readonly SpaceDirection LEFT_DOWN = new SpaceDirection(HorizontalDirection.LEFT, VerticalDirection.DOWN);
		static readonly SpaceDirection NONE_DOWN = new SpaceDirection(HorizontalDirection.NONE, VerticalDirection.DOWN);
		static readonly SpaceDirection RIGHT_DOWN = new SpaceDirection(HorizontalDirection.RIGHT, VerticalDirection.DOWN);
		static readonly SpaceDirection LEFT_NONE = new SpaceDirection(HorizontalDirection.LEFT, VerticalDirection.NONE);
		static readonly SpaceDirection NONE_NONE = new SpaceDirection(HorizontalDirection.NONE, VerticalDirection.NONE);
		static readonly SpaceDirection RIGHT_NONE = new SpaceDirection(HorizontalDirection.RIGHT, VerticalDirection.NONE);

		public HorizontalDirection Horizontal
		{
			get; protected set;
		}
		public VerticalDirection Vertical
		{
			get; protected set;
		}

		//Returns an empty direction set
		public static SpaceDirection None
		{
			get
			{
				return NONE_NONE;
			}
		}

		/*
		//Destroyer operator, a custom reset operation, returns an empty direction set
		public static SpaceDirection operator~ (SpaceDirection oldDirection)
		{
			return NONE_NONE;
		}
		*/

		SpaceDirection(HorizontalDirection horizontal, VerticalDirection vertical)
		{
			Horizontal = horizontal;
			Vertical = vertical;
		}

		//Direction seems a good reason for using a singleton
		public static SpaceDirection get(HorizontalDirection horizontal, VerticalDirection vertical)
		{
			if (horizontal == HorizontalDirection.LEFT)
			{
				return vertical==VerticalDirection.UP ? LEFT_UP : vertical == VerticalDirection.DOWN ? LEFT_DOWN : LEFT_NONE;
			}
			else if (horizontal == HorizontalDirection.RIGHT)
			{
				return vertical == VerticalDirection.UP? RIGHT_UP : vertical == VerticalDirection.DOWN ? RIGHT_DOWN : RIGHT_NONE;
			}
			else
			{
				return vertical == VerticalDirection.UP ? NONE_UP : vertical == VerticalDirection.DOWN ? NONE_DOWN : NONE_NONE;
			}
		}

		//Overloads for convenience
		public static SpaceDirection get(HorizontalDirection horizontal)
		{
			return get(horizontal, VerticalDirection.NONE);
		}
		public static SpaceDirection get(VerticalDirection vertical)
		{
			return get(HorizontalDirection.NONE, vertical);
		}

		//Conversion from angle
		public static SpaceDirection getFromAngle(float angle)
		{
			angle = Utility.NormalizeDegreeAngle(angle);
			if (angle > 0 && angle < 90)
			{
				return SpaceDirection.get(SpaceDirection.HorizontalDirection.RIGHT, SpaceDirection.VerticalDirection.UP);
			}
			else if (angle == 90)
			{
				return SpaceDirection.get(SpaceDirection.HorizontalDirection.RIGHT);
			}
			else if (angle > 90 && angle < 180)
			{
				return SpaceDirection.get(SpaceDirection.HorizontalDirection.RIGHT, SpaceDirection.VerticalDirection.DOWN);
			}
			else if (angle == 180)
			{
				return SpaceDirection.get(SpaceDirection.VerticalDirection.DOWN);
			}
			else if (angle > 180 && angle < 270)
			{
				return SpaceDirection.get(SpaceDirection.HorizontalDirection.LEFT, SpaceDirection.VerticalDirection.DOWN);
			}
			else if (angle == 270)
			{
				return SpaceDirection.get(SpaceDirection.HorizontalDirection.LEFT);
			}
			else if (angle > 270 && angle < 360)
			{
				return SpaceDirection.get(SpaceDirection.HorizontalDirection.LEFT, SpaceDirection.VerticalDirection.UP);
			}
			else if (angle == 360 || angle == 0)
			{
				return SpaceDirection.get(SpaceDirection.VerticalDirection.UP);
			}
			throw new Exception("Fatal error: Angle for conversion in SpaceDirection was not normalized.");
		}

		public static SpaceDirection operator +(SpaceDirection direction, HorizontalDirection horizontal)
		{
			//Opposite directions cancel out
			if
				(
				horizontal == HorizontalDirection.LEFT && direction.Horizontal == HorizontalDirection.RIGHT
				||
				horizontal == HorizontalDirection.RIGHT && direction.Horizontal == HorizontalDirection.LEFT
				)
			{
				return get(HorizontalDirection.NONE, direction.Vertical);
			}
			else
			{
				return get(horizontal, direction.Vertical);
			}
		}

		public static SpaceDirection operator +(SpaceDirection direction, VerticalDirection vertical)
		{
			//Opposite directions cancel out
			if
				(
				vertical == VerticalDirection.UP && direction.Vertical == VerticalDirection.DOWN
				||
				vertical == VerticalDirection.DOWN && direction.Vertical == VerticalDirection.UP
				)
			{
				return get(direction.Horizontal, VerticalDirection.NONE);
			}
			else
			{
				return get(direction.Horizontal, vertical);
			}
		}

		public static SpaceDirection operator -(SpaceDirection direction, HorizontalDirection horizontal)
		{
			//Same directions cancel out
			if (direction.Horizontal == horizontal)
			{
				return get(HorizontalDirection.NONE, direction.Vertical);
			}
			//Direction can be reversed using unary minus operator
			else if (direction.Horizontal == HorizontalDirection.NONE && horizontal == HorizontalDirection.LEFT)
			{
				return get(HorizontalDirection.RIGHT, direction.Vertical);
			}
			else if (direction.Horizontal == HorizontalDirection.NONE && horizontal == HorizontalDirection.RIGHT)
			{
				return get(HorizontalDirection.LEFT, direction.Vertical);
			}
			//Adding two directions (double negation) is a neutral operation
			else
			{
				return get(direction.Horizontal, direction.Vertical);
			}
		}

		public static SpaceDirection operator -(SpaceDirection direction, VerticalDirection vertical)
		{
			//Same directions cancel out
			if (direction.Vertical == vertical)
			{
				return get(direction.Horizontal, VerticalDirection.NONE);
			}
			//Direction can be reversed using unary minus operator
			else if (direction.Vertical == VerticalDirection.NONE && vertical == VerticalDirection.DOWN)
			{
				return get(direction.Horizontal, VerticalDirection.UP);
			}
			else if (direction.Vertical == VerticalDirection.NONE && vertical == VerticalDirection.UP)
			{
				return get(direction.Horizontal, VerticalDirection.DOWN);
			}
			//Adding two directions (double negation) is a neutral operation
			else
			{
				return get(direction.Horizontal, direction.Vertical);
			}
		}

		/*
		public static bool operator ==(Direction direction1, Direction direction2)
		{
				return direction1.Horizontal == direction2.Horizontal && direction1.Vertical == direction2.Vertical;
		}

		public static bool operator !=(Direction direction1, Direction direction2)
		{
			return direction1.Horizontal != direction2.Horizontal || direction1.Vertical != direction2.Vertical;
		}
		*/

		public override string ToString()
		{
			return Horizontal.ToString() + " " + Vertical.ToString();
		}
	}
}
