using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.IntersectStrategy
{
	public interface IIntersectStrategy
	{
		bool IntersectsActor(IActor actor);
		bool IntersectsOn(float x, float y);
		bool IntersectsWithin(float x, float width, float y, float height);
	}
}
