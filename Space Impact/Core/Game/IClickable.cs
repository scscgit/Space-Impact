using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game
{
	/// <summary>
	/// Adds an ability to be clicked on.
	/// </summary>
	public interface IClickable
	{
		bool Clicked { get; }

		void Click(float x, float y);
		void ClickMove(float x, float y);
		void ClickRelease();
	}
}
