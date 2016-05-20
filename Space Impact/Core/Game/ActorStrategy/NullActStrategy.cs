using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.ActorStrategy
{
	/// <summary>
	/// Does nothing. Null pattern. Looks nice.
	/// </summary>
	class NullActStrategy : IActStrategy
	{
		public void Act()
		{
		}
	}
}
