using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Space_Impact.Core.Game;
using Space_Impact.Support;

namespace Space_Impact.Core.Graphics.Background.Strategy
{
	public class RandomMovement : IBackgroundStrategy
	{
		IBackground Background;
		SpaceDirection Direction = SpaceDirection.None;

		float LeftMax = 0;
		float RightMax = 0;
		float TopMax = 0;
		float BottomMax = 0;

		public RandomMovement(IBackground background, IField field)
		{
			Background = background;
			OnAnimationSet(field);
		}

		public void DrawHook(CanvasDrawingSession draw)
		{
			switch (Direction.Horizontal)
			{
				case SpaceDirection.HorizontalDirection.LEFT:
					moveX(Background.X - Background.Speed);
					break;
				case SpaceDirection.HorizontalDirection.RIGHT:
					moveX(Background.X + Background.Speed);
					break;
			}

			switch (Direction.Vertical)
			{
				case SpaceDirection.VerticalDirection.UP:
					moveY(Background.Y - Background.Speed);
					break;
				case SpaceDirection.VerticalDirection.DOWN:
					moveY(Background.Y + Background.Speed);
					break;
			}
		}

		private void moveX(float x)
		{
			if (Direction.Horizontal.Equals(SpaceDirection.HorizontalDirection.LEFT) && x < LeftMax)
			{
				Direction = SpaceDirection.get(SpaceDirection.HorizontalDirection.RIGHT, Direction.Vertical);
			}
			else if (Direction.Horizontal.Equals(SpaceDirection.HorizontalDirection.RIGHT) && x > RightMax)
			{
				Direction = SpaceDirection.get(SpaceDirection.HorizontalDirection.LEFT, Direction.Vertical);
			}
			else
			{
				Background.X = x;
			}
		}

		private void moveY(float y)
		{
			if (Direction.Vertical.Equals(SpaceDirection.VerticalDirection.UP) && y < TopMax)
			{
				Direction = SpaceDirection.get(Direction.Horizontal, SpaceDirection.VerticalDirection.DOWN);
			}
			else if (Direction.Vertical.Equals(SpaceDirection.VerticalDirection.DOWN) && y > BottomMax)
			{
				Direction = SpaceDirection.get(Direction.Horizontal, SpaceDirection.VerticalDirection.UP);
			}
			else
			{
				Background.Y = y;
			}
		}

		public void OnAnimationSet(IField field)
		{
			//Calculating borders
			LeftMax = (float)field.Size.Width - (float)Background.Width;
			TopMax = (float)field.Size.Height - (float)Background.Height;
			Log.i(this, "Background borders: LeftMax = = " + RightMax+ ", TopMax = " + BottomMax);

			//Initializing position as a random one
			Background.X = Utility.RandomBetween((int)LeftMax, (int)RightMax);
			Background.Y = Utility.RandomBetween((int)TopMax, (int)BottomMax);

			//Initializing random direction
			int horizontalEnum = Utility.RandomBetween(0, 2);
			int verticalEnum = Utility.RandomBetween(0, 2);
			SpaceDirection.HorizontalDirection horizontal;
			SpaceDirection.VerticalDirection vertical;

			//Excluding NONE direction
			//(bias will be towards horizontal direction, because he has too much of the vertical one during the gameplay)
			if (horizontalEnum == 0 && verticalEnum == 0)
			{
				horizontalEnum = Utility.RandomBetween(1, 2);
			}

			switch (horizontalEnum)
			{
				case 0:
					horizontal = SpaceDirection.HorizontalDirection.NONE;
					break;
				case 1:
					horizontal = SpaceDirection.HorizontalDirection.LEFT;
					break;
				case 2:
				default:
					horizontal = SpaceDirection.HorizontalDirection.RIGHT;
					break;
			}

			switch (verticalEnum)
			{
				case 0:
					vertical = SpaceDirection.VerticalDirection.NONE;
					break;
				case 1:
					vertical = SpaceDirection.VerticalDirection.UP;
					break;
				case 2:
				default:
					vertical = SpaceDirection.VerticalDirection.DOWN;
					break;
			}

			Direction = SpaceDirection.get(horizontal, vertical);
		}
	}
}
