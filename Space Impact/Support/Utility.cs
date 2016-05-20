using Space_Impact.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Support
{
	public static class Utility
	{
		public static float NormalizeAngle(float angle)
		{
			angle %= 360;
			if (angle < 0)
			{
				angle += 360;
			}
			return angle;
		}

		
	}
}
