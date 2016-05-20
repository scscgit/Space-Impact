using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Windows.UI;
using Space_Impact.Support;

namespace Space_Impact.Core.Game.Character
{
	public abstract class AbstractCharacter: AbstractActor, ICharacter
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
					if(Health> value)
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
				if(value>MaxHealth)
				{
					this.health = MaxHealth;
				}
				else if (value > 0 && value <= MaxHealth)
				{
					this.health = value;
				}
				else
				{
					this.health = 0;
					OnDeath();
				}
			}
		}

		protected AbstractCharacter(string name) : base(name)
		{

		}

		public abstract void OnDeath();

		protected override void DrawAbstractModification(ref ICanvasImage bitmap, CanvasDrawingSession draw)
		{
			base.DrawAbstractModification(ref bitmap, draw);
			
			//Draws a healthbar above the character
			var rectangleHeight = Width/30;

			var rect = new Windows.Foundation.Rect();
			rect.X = X;
			rect.Width = Width;
			rect.Y = Y - rectangleHeight;
			rect.Height = rectangleHeight;

			int healthPercent = Health * 100 / MaxHealth;

			ICanvasBrush brush;
			if (healthPercent>80)
			{
				brush = new CanvasSolidColorBrush(draw, Colors.Green);
			}
			else if(healthPercent > 30)
			{
				brush = new CanvasSolidColorBrush(draw, Colors.Yellow);
			}
			else
			{
				brush = new CanvasSolidColorBrush(draw, Colors.Red);
			}

			draw.DrawRoundedRectangle(rect, 5, 5, brush);

			rect.Width = ((float)healthPercent/100) * Width;

			draw.FillRoundedRectangle(rect, 5, 5, brush);
		}
	}
}
