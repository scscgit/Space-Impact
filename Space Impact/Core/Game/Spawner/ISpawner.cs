using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Spawner
{
	/// <summary>
	/// Spawner is also like an Actor, but does not implement Actor interface, only Act.
	/// </summary>
	public interface ISpawner: IAct
	{
		IField Field { get; }
		int RemainingEnemies { get; }
		bool UnlimitedEnemies { get; }
		bool Active { get; }

		void Spawn();
	}
}
