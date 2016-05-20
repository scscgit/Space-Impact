using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Space_Impact.Support;
using Windows.Foundation;

namespace Space_Impact.Graphics
{
	public abstract class AbstractAnimatedObject : IAnimatedObject
	{
		protected AbstractAnimatedObject()
		{
			AnimationSpeed = 1;
		}
		
		//Location on a field, TODO don't forget C# isn't virtual implicitly
		public float X
		{
			get; set;
		} = 0;
		public float Y
		{
			get; set;
		} = 0;
		public double Width
		{
			get; protected set;
		} = 0;
		public double Height
		{
			get; protected set;
		} = 0;

		//Current frame of the animation
		public int Frame
		{
			get; protected set;
		} = 0;

		//Speed (duration) of animation, lower is faster
		int FrameIndex = 0;
		int animationSpeed;
		public int AnimationSpeed
		{
			protected get
			{
				return animationSpeed;
			}
			set
			{
				animationSpeed = value > 0 ? value : 1;
			}
		}

		/// <summary>
		/// Prepares a texture set to be loaded as bitmaps for representing an animated object.
		/// Only relative path from Assets folder is required, .png is NOT automatically added.
		/// </summary>
		/// <param name="textureSet">Texture set containing names (paths) of .png or .jpg (or other) files inside Assets directory.
		/// Supported null for no animation.</param>
		string[] textures = null;
		CanvasBitmap[] bitmaps = null;
		public string[] Animation
		{
			set
			{
				this.textures = value;

				//Null parameter disables animation
				if (value == null)
				{
					this.bitmaps = null;
					return;
				}

				this.bitmaps = TextureSetLoader.Instance[value];
				Frame = 0;

				//Updates current description variables
				var size = this.bitmaps[Frame].Size;
				Width = size.Width;
				Height = size.Height;

				//Lets subclasses hook some event after a new animation is set
				OnAnimationSet();
			}
			protected get
			{
				return textures;
			}
		}

		/// <summary>
		/// Lets subclasses hook some event after a new animation is set.
		/// </summary>
		protected virtual void OnAnimationSet()
		{
		}

		/// <summary>
		/// Draws a texture on the screen using the current X and Y coordinates.
		/// Increments Frame counters.
		/// </summary>
		/// <param name="draw">Session for drawing on the current frame</param>
		public void Draw(CanvasDrawingSession draw)
		{
			//Increments Frame counter
			FrameIndex++;
			if (FrameIndex >= AnimationSpeed)
			{
				FrameIndex = 0;
				Frame++;
				if (Frame >= this.textures.GetLength(0) || Frame >= this.bitmaps.GetLength(0))
				{
					Frame = 0;
				}
			}

			//Prepares the bitmap to be drawn
			ICanvasImage bitmap = this.bitmaps[Frame];

			//Lets abstract clases hook the bitmap modification for general purposes
			DrawAbstractModification(ref bitmap, draw);

			//Lets subclasses hook the bitmap modification
			DrawModification(ref bitmap, draw);

			if (bitmap == null)
			{
				return;
			}

			//Draw operation delegation to the Canvas Session on the current coordinates
			draw.DrawImage(bitmap, new Vector2(this.X, this.Y));

			//Updates current description variables
			var size = this.bitmaps[Frame].Size;
			Width = size.Width;
			Height = size.Height;

			DrawHook(draw);
		}

		/// <summary>
		/// Optional draw operations at the end of Draw().
		/// Can be used to call Draw() over a composition of objects.
		/// </summary>
		/// <param name="draw">Session for drawing on the current frame</param>
		protected virtual void DrawHook(CanvasDrawingSession draw)
		{
		}

		/// <summary>
		/// Hook for changing bitmap by returning changed version to the Draw operation on Canvas
		/// Used by abstract classes
		/// </summary>
		/// <param name="bitmap">Canvas Bitmap supposed to be drawn on the Canvas</param>
		/// <returns>Canvas Bitmap actually drawn on the Canvas</returns>
		protected virtual void DrawAbstractModification(ref ICanvasImage bitmap, CanvasDrawingSession draw)
		{
		}

		/// <summary>
		/// Hook for changing bitmap by returning changed version to the Draw operation on Canvas
		/// </summary>
		/// <param name="bitmap">Canvas Bitmap supposed to be drawn on the Canvas</param>
		/// <returns>Canvas Bitmap actually drawn on the Canvas</returns>
		protected virtual void DrawModification(ref ICanvasImage bitmap, CanvasDrawingSession draw)
		{
		}
	}
}
