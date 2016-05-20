using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game
{
	/// <summary>
	/// Adds ability to be clicked on.
	/// </summary>
	interface IClickable
	{
		void Click(float x, float y);
		void ClickMove(float x, float y);
		void ClickRelease();
	}
}
