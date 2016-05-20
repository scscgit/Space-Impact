using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.ActorStrategy
{
	/// <summary>
	/// Common interface for all kinds of strategies.
	/// 
	/// Advised usage: use is operator to check for concrete type of strategy before casting and calling it.
	/// </summary>
	public interface IStrategy
	{
	}
}
