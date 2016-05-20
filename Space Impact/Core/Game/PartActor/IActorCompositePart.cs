using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.PartActor
{
	/// <summary>
	/// A.K.A "Part"
	/// </summary>
	public interface IActorCompositePart : IActor, IAnimatedObject
	{
		//void RegisterPartIn(IActor containedInActor);
		void UnregisterPart();
	}
}
