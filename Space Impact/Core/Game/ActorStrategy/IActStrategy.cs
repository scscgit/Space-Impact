using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.ActorStrategy
{
	/// <summary>
	/// Supports modification of the Act content.
	/// </summary>
	public interface IActStrategy : IStrategy
	{
		void Act();
	}
}
