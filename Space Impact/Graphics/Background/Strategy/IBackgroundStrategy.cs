using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Graphics.Background.Strategy
{
	public interface IBackgroundStrategy
	{
		void OnAnimationSet(IField field);
		void DrawHook(CanvasDrawingSession draw);
	}
}
