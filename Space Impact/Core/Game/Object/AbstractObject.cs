using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Object
{
	public abstract class AbstractObject: AbstractActor, IObject
	{
		protected AbstractObject(string name): base(name)
		{

		}
	}
}
