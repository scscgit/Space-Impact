using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Object.Collectable
{
	public abstract class AbstractCollectable : AbstractObject, ICollectable
	{
		protected AbstractCollectable(String name) : base(name)
		{

		}


	}
}
