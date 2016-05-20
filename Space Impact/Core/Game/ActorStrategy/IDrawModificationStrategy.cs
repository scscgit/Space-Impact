using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.ActorStrategy
{
	/// <summary>
	/// Supports modification of image.
	/// </summary>
	public interface IDrawModificationStrategy : IStrategy
	{
		/// <summary>
		/// Modifies image before it is used for drawing.
		/// bitmap should be replaced by the returned bitmap.
		/// </summary>
		/// <param name="bitmap">Current bitmap as an input-output variable</param>
		/// <param name="draw">Access to the current drawing session for possible special needs</param>
		void DrawModification(ref ICanvasImage bitmap, CanvasDrawingSession draw);
	}
}
