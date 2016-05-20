using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core
{
	public interface IPlacedOnField
	{
		float X { get; set; }
		float Y { get; set; }
		double Width { get; }
		double Height { get; }
	}
}
