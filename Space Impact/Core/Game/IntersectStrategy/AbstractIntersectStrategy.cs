using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.IntersectStrategy
{
	public abstract class AbstractIntersectStrategy : IIntersectStrategy
	{
		protected IPlacedOnField Actor
		{
			get; private set;
		}

		public AbstractIntersectStrategy(IPlacedOnField actor)
		{
			Actor = actor;
		}

		/// <summary>
		/// By default, intersects actor using IntersectsWithin function.
		/// </summary>
		/// <param name="actor">actor, which is checked for intersection against the current Actor</param>
		/// <returns></returns>
		public virtual bool IntersectsActor(IActor actor)
		{
			return actor.IntersectsWithin(Actor.X, (float)Actor.Width, Actor.Y, (float)Actor.Height);
		}

		public abstract bool IntersectsOn(float x, float y);

		public abstract bool IntersectsWithin(float x, float width, float y, float height);
	}
}
