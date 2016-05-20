using Space_Impact.Core.Graphics.Background.Strategy;
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
		void AddStrategy(IBackgroundStrategy strategy);
		float Speed { get; set; }
	}
}
