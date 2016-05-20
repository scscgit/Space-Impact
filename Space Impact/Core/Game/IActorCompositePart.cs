using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game
{
	/// <summary>
	/// A.K.A "Part"
	/// </summary>
	public interface IActorCompositePart : IActor
	{
		void RegisterPartIn(IActor containedInActor);
		void UnregisterPart();
	}
}
