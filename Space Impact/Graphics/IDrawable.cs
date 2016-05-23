using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Graphics
{
	public interface IDrawable
	{
		void Draw(CanvasDrawingSession draw);
	}
}
