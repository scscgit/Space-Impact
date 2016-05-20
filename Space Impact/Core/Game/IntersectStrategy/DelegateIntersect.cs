using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.IntersectStrategy
{
	/// <summary>
	/// Delegate implementation for easy integration of ad-hoc Intersect Strategy into any Actor class.
	/// </summary>
	public class DelegateIntersect : AbstractIntersectStrategy
	{
		public delegate bool IntersectsActorDelegate(IActor actor);
		public delegate bool IntersectsOnDelegate(float x, float y);
		public delegate bool IntersectsWithinDelegate(float x, float width, float y, float height);

		IntersectsActorDelegate IntersectsActorCurrent;
		IntersectsOnDelegate IntersectsOnCurrent;
		IntersectsWithinDelegate IntersectsWithinCurrent;

		bool IntersectsWithinEachPixel(float x_start, float width, float y_start, float height)
		{
			for (float x = x_start; x < x_start + width; x++)
			{
				for (float y = y_start; y < y_start + height; y++)
				{
					if (IntersectsOn(x, y))
					{
						return true;
					}
				}
			}
			return false;
		}

		public DelegateIntersect(IPlacedInSpace actor, IntersectsActorDelegate intersectsActor, IntersectsOnDelegate intersectsOn, IntersectsWithinDelegate intersectsWithin) : base(actor)
		{
			IntersectsActorCurrent = intersectsActor;
			IntersectsOnCurrent = intersectsOn;
			IntersectsWithinCurrent = intersectsWithin;
		}

		public DelegateIntersect(IPlacedInSpace actor, IntersectsOnDelegate intersectsOn): base(actor)
		{
			IntersectsActorCurrent = base.IntersectsActor;
			IntersectsOnCurrent = intersectsOn;
			IntersectsWithinCurrent = IntersectsWithinEachPixel;
		}

		public override bool IntersectsActor(IActor actor)
		{
			return IntersectsActorCurrent(actor);
		}

		public override bool IntersectsOn(float x, float y)
		{
			return IntersectsOnCurrent(x, y);
		}

		public override bool IntersectsWithin(float x, float width, float y, float height)
		{
			return IntersectsWithinCurrent(x, width, y, height);
		}
	}
}
