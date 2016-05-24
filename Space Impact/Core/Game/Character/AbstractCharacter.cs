using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Windows.UI;
using Space_Impact.Support;
using Space_Impact.Core.Game.Weapon;

namespace Space_Impact.Core.Game.Character
{
	public abstract class AbstractCharacter : AbstractActor, ICharacter
	{
		//Constants
		const int DEFAULT_MAX_HEALTH = 100;

		//Properties
		int maxHealth = DEFAULT_MAX_HEALTH;
		public int MaxHealth
		{
			get
			{
				return this.maxHealth;
			}

			protected set
			{
				if (value > 0)
				{
					this.maxHealth = value;

					//Cannot have more Health than the MaxHealth
					if (Health > value)
					{
						Health = value;
					}
				}
			}
		}

		int health = DEFAULT_MAX_HEALTH;
		public int Health
		{
			get
			{
				return this.health;
			}

			set
			{
				if (value > MaxHealth)
				{
					this.health = MaxHealth;
				}
				else if (value > 0 && value <= MaxHealth)
				{
					this.health = value;
				}
				//If the health is already on zero, we do not trigger another death
				else if (this.health > 0)
				{
					this.health = 0;
					OnDeath();
				}
			}
		}

		/// <summary>
		/// Weapon of the character, used by default implementation for shooting projectiles.
		/// </summary>
		private IWeapon weapon;
		public IWeapon Weapon
		{
			get
			{
				return weapon;
			}
			set
			{
				weapon = value;
			}
		}

		protected AbstractCharacter(string name) : base(name)
		{
			Weapon = null;
		}

		public abstract void OnDeath();

		protected override void DrawAbstractModification(ref ICanvasImage bitmap, CanvasDrawingSession draw)
		{
			base.DrawAbstractModification(ref bitmap, draw);

			//Debugging information about characters
			if (Utility.SettingsLoad<bool>("debug") && this is IAngle)
			{
				//Current angle in degrees
				draw.DrawText
				(
					(int)(this as IAngle).Angle + "°"
					, new System.Numerics.Vector2(X + (float)Width - 30, Y + (float)Height / 2)
					, Colors.FloralWhite
				);
			}

			//Draws a healthbar above the character
			var rectangleHeight = Width / 30;

			var rect = new Windows.Foundation.Rect();
			rect.X = X;
			rect.Width = Width;
			rect.Y = Y - rectangleHeight;
			rect.Height = rectangleHeight;

			int healthPercent = Health * 100 / MaxHealth;

			ICanvasBrush brush;
			if (healthPercent > 80)
			{
				brush = new CanvasSolidColorBrush(draw, Colors.Green);
			}
			else if (healthPercent > 30)
			{
				brush = new CanvasSolidColorBrush(draw, Colors.Yellow);
			}
			else
			{
				brush = new CanvasSolidColorBrush(draw, Colors.Red);
			}

			draw.DrawRoundedRectangle(rect, 5, 5, brush);

			rect.Width = ((float)healthPercent / 100) * Width;

			draw.FillRoundedRectangle(rect, 5, 5, brush);
		}

		/// <summary>
		/// By default, any Character may not be able to shoot, but is obligated to implement this function and return false.
		/// </summary>
		/// <returns>true if the shot was successful</returns>
		protected virtual bool Shot()
		{
			return false;
		}
	}
}
