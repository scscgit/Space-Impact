using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game
{
	public delegate bool CollidesOn(float x, float y);

	/// <summary>
	/// Implements clickability support.
	/// </summary>
	public class ClickableImpl : IClickable
	{
		public bool Clicked
		{
			get; private set;
		} = false;

		CollidesOn CollidesOn;
		Position LastClick;

		/// <summary>
		/// Implements clickability support for any IPlacedInSpace object.
		/// </summary>
		/// <param name="collidesOn">Delegate that checks if a click on the coordinates causes the object to become clicked</param>
		public ClickableImpl(CollidesOn collidesOn)
		{
			this.CollidesOn = collidesOn;
		}

		public void Click(float x, float y)
		{
			LastClick.X = x;
			LastClick.Y = y;
			if (this.CollidesOn(x, y))
			{
				Clicked = true;
			}
		}

		public void ClickMove(float x, float y)
		{
			LastClick.X = x;
			LastClick.Y = y;
			if (Clicked && !this.CollidesOn(x, y))
			{
				Clicked = false;
			}
		}

		public void ClickRelease()
		{
			if (Clicked)
			{
				Clicked = false;
			}
		}

		/// <summary>
		/// Supporting periodical operations required for the proper functioning of Clicked field.
		/// </summary>
		public void Act()
		{
			//If the user does not move the mouse, but it is still Clicked, last click position is used to check for the validity of the click
			if (Clicked && !this.CollidesOn(LastClick.X, LastClick.Y))
			{
				Clicked = false;
			}
		}
	}
}
