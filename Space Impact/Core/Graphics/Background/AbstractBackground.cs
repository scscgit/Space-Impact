using Microsoft.Graphics.Canvas;
using Space_Impact.Graphics;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Graphics.Background
{
	/// <summary>
	/// The art of abstraction.
	/// </summary>
	public class AbstractBackground : AbstractAnimatedObject, IBackground
	{
		protected IField Field
		{
			get; set;
		}

		/// <summary>
		/// Speed of animation movement
		/// </summary>
		public float Speed
		{
			get; set;
		}

		/// <summary>
		/// Percentage of the finished part of the background (map)
		/// </summary>
		public float Percent
		{
			get
			{
				return 100 - ((Y / (-(float)Height + (float)Field.Size.Height)) * 100);
			}
		}

		public AbstractBackground(IField field)
		{
			Field = field;
			Speed = 1;
		}

		protected override void OnAnimationSet()
		{
			base.OnAnimationSet();
			Y = (float)-Height + (float)Field.Size.Height;
		}

		//Act() of the background
		protected override void DrawHook(CanvasDrawingSession draw)
		{
			base.DrawHook(draw);
			if (Y < 0)
			{
				if (Y + Speed >= 0)
				{
					Y = 0;
				}
				else
				{
					Y += Speed;
				}
			}
		}
	}
}
