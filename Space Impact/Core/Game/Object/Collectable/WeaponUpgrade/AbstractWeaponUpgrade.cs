using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Windows.UI;
using Microsoft.Graphics.Canvas.Brushes;
using Space_Impact.Core.Game.Player;
using Space_Impact.Core.Game.Weapon;
using Space_Impact.Support;

namespace Space_Impact.Core.Game.Object.Collectable.WeaponUpgrade
{
	public abstract class AbstractWeaponUpgrade : AbstractCollectable, IWeaponUpgrade
	{
		public IWeapon Weapon
		{
			get; protected set;
		}

		protected AbstractWeaponUpgrade(string name) : base(name)
		{
		}

		protected override void DrawAbstractModification(ref ICanvasImage bitmap, CanvasDrawingSession draw)
		{
			base.DrawAbstractModification(ref bitmap, draw);

			var rect = new Windows.Foundation.Rect();
			rect.X = X - Width * 0.25;
			rect.Y = Y - Height * 0.25;
			rect.Width = Width * 1.5;
			rect.Height = Height * 1.5;
			draw.DrawRoundedRectangle(rect, (float)Width, (float)Height, new CanvasSolidColorBrush(draw, BorderColor()), 5);
		}

		/// <summary>
		/// Color displayed as the border of the Weapon Upgrade.
		/// </summary>
		/// <returns>Random Color meant to be used in the next Draw cycle</returns>
		private Color BorderColor()
		{
			Color[] colors =
			{
				Colors.SkyBlue, Colors.SeaGreen, Colors.YellowGreen
			};
			return colors[Utility.RandomBetween(0, colors.Length - 1)];
		}

		public override void CollectBy(IPlayer player)
		{
			if (Weapon != null)
			{
				player.Weapon = Weapon;
				Weapon = null;
			}
			RemoveFromField();
		}
	}
}
