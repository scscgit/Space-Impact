using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Spawner.Wrapper
{
	/// <summary>
	/// Hides the Act, modifying behavior of a Spawner by providing an external control over calling the real Act().
	/// </summary>
	public class HideAct: AbstractSpawnerWrapper
	{
		public HideAct(ISpawner spawner) : base(spawner)
		{
		}

		public override void Act()
		{
		}

		public void HiddenAct()
		{
			base.Act();
		}
	}
}
