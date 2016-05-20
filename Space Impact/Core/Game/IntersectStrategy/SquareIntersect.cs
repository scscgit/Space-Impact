using Space_Impact.Graphics;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.IntersectStrategy
{
	public class SquareIntersect : AbstractIntersectStrategy
	{
		/// <summary>
		/// Percentage as a coefficient in the interval of <0, 1> when Percent is between <0, 100>.
		/// </summary>
		private float PercentCoefficient;
		/// <summary>
		/// Percentage size of the Actor's animation that is used for intersect calculation.
		/// </summary>
		public float Percent
		{
			get
			{
				return PercentCoefficient * 100;
			}
			set
			{
				PercentCoefficient = value / 100;
			}
		}

		public SquareIntersect(IPlacedInSpace actor) : base(actor)
		{
			Percent = 100;
		}

		public override bool IntersectsOn(float x, float y)
		{
			return IntersectsWithin(x, 1, y, 1);
		}

		float XLeft()
		{
			return Actor.X + ((1 - PercentCoefficient) / 2) * (float)Actor.Width;
		}
		float XRight()
		{
			return XLeft() + PercentCoefficient * (float)Actor.Width;
		}
		float YTop()
		{
			return Actor.Y + ((1 - PercentCoefficient) / 2) * (float)Actor.Height;
		}
		float YBottom()
		{
			return YTop() + PercentCoefficient * (float)Actor.Height;
		}

		public override bool IntersectsWithin(float x, float width, float y, float height)
		{
			return
				(x + width >= XLeft() && x <= XRight() &&
				y + height >= YTop() && y <= YBottom());
		}
	}
}
