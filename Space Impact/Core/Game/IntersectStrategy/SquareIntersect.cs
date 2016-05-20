using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.IntersectStrategy
{
	public class SquareIntersect: AbstractIntersectStrategy
	{
		//
		//TODO impleement percentage decreasing

		//
		//
		public float Percentage = 100;

		public SquareIntersect(IPlacedOnField actor): base(actor)
		{
		}

		public override bool IntersectsOn(float x, float y)
		{
			return IntersectsWithin(x, 0, y, 0);
		}

		public override bool IntersectsWithin(float x, float width, float y, float height)
		{
			return
				(x + width >= Actor.X && x <= Actor.X + Actor.Width &&
				y + height >= Actor.Y && y <= Actor.Y + Actor.Height);
		}
	}
}
