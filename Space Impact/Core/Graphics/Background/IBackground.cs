using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Graphics.Background
{
	public interface IBackground : IAnimatedObject
	{
		float Speed { get; set; }
		float Percent { get; }
	}
}
