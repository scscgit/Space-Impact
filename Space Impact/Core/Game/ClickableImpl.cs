using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game
{
	public delegate bool CollidesOn(float x, float y);

	/// <summary>
	/// Implements clickability support
	/// </summary>
	public class ClickableImpl: IClickable
	{
		public bool Clicked
		{
			get; private set;
		} = false;
		CollidesOn collidesOn;

		/// <summary>
		/// Implements clickability support
		/// </summary>
		/// <param name="collidesOn">Delegate that checks if click on coordinates causes the object to become clicked</param>
		public ClickableImpl(CollidesOn collidesOn)
		{
			this.collidesOn = collidesOn;
		}

		public void Click(float x, float y)
		{
			if (this.collidesOn(x, y))
			{
				Clicked = true;
			}
		}

		public void ClickMove(float x, float y)
		{
			if(Clicked && !this.collidesOn(x, y))
			{
				Clicked = false;
			}
		}

		public void ClickRelease()
		{
			if(Clicked)
			{
				Clicked = false;
			}
		}
	}
}
