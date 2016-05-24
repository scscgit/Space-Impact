using Microsoft.Graphics.Canvas;
using Space_Impact.Core.Graphics.Background.Strategy;
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
	public abstract class AbstractBackground : AbstractAnimatedObject, IBackground
	{
		LinkedList<IBackgroundStrategy> Strategies;

		/// <summary>
		/// Field under which the background is displayed.
		/// </summary>
		public IField Field
		{
			get; private set;
		}

		/// <summary>
		/// Speed of the background animation movement.
		/// </summary>
		public float Speed
		{
			get; set;
		}

		protected AbstractBackground(IField field)
		{
			Strategies = new LinkedList<IBackgroundStrategy>();
			Field = field;
			Speed = 1;
		}

		public void AddStrategy(IBackgroundStrategy strategy)
		{
			Strategies.AddLast(strategy);
		}

		protected override void OnAnimationSet()
		{
			base.OnAnimationSet();
			foreach (IBackgroundStrategy strategy in Strategies)
			{
				strategy.OnAnimationSet(Field);
			}
		}

		//Act() of the background
		protected override void DrawHook(CanvasDrawingSession draw)
		{
			base.DrawHook(draw);
			foreach (IBackgroundStrategy strategy in Strategies)
			{
				strategy.DrawHook(draw);
			}
		}
	}
}
