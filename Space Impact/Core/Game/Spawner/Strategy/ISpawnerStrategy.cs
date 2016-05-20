using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Spawner.Strategy
{
	public interface ISpawnerStrategy: IAct
	{
		void Act();
	}
}
