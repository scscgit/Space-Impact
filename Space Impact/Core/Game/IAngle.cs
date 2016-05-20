using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game
{
	/// <summary>
	/// Supports angular movement.
	/// </summary>
	public interface IAngle
	{
		/// <summary>
		/// Angle in degrees.
		/// </summary>
		float Angle
		{
			get; set;
		}
	}
}
