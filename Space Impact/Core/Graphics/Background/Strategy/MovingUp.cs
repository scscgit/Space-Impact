using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Space_Impact.Support;

namespace Space_Impact.Core.Graphics.Background.Strategy
{
	/// <summary>
	/// Moves up at the Speed of the Background from below.
	/// </summary>
	public class MovingUp : IBackgroundStrategy
	{
		protected IBackground Background
		{
			get; private set;
		}

		/// <summary>
		/// Static function:
		/// Returns a percentage completion of a Background within a Field, that is using MovingUp strategy.
		/// </summary>
		public static float PercentageCompletion(IBackground background, IField field)
		{
			return 100 - ((background.Y / (-(float)background.Height + (float)field.Size.Height)) * 100);
		}

		public MovingUp(IBackground background, IField field)
		{
			Background = background;
			OnAnimationSet(field);
		}

		public void DrawHook(CanvasDrawingSession draw)
		{
			if (Background.Y < 0)
			{
				if (Background.Y + Background.Speed >= 0)
				{
					Background.Y = 0;
				}
				else
				{
					Background.Y += Background.Speed;
				}
			}
		}

		public void OnAnimationSet(IField field)
		{
			Background.Y = (float)-Background.Height + (float)field.Size.Height;
		}
	}
}
