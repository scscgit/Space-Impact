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
	public class AnimatedObject : IAnimatedObject
	{
		public AnimatedObject()
		{
			AnimationSpeed = 1;
		}

		//Location on a field, TODO don't forget C# isnt virtual implicitly
		public int X
		{
			get; protected set;
		} = 0;
		public int Y
		{
			get; protected set;
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
		private int FrameIndex = 0;
		private int animationSpeed;
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

		//Current drawing Canvas Session, args.DrawingSession of XAML canvas
		public CanvasDrawingSession DrawingSession
		{
			protected get; set;
		}

		//Current Canvas Control, sender of XAML canvas
		public ICanvasAnimatedControl CanvasControlSender
		{
			protected get; set;
		}

		private string[] textures = null;
		private CanvasBitmap[] bitmaps = null;

		//public AnimatedObject(string[] textures)
		//{
		//	setAnimation(textures);
		//}

		/// <summary>
		/// Prepares a texture set to be loaded as bitmaps for representing an animated object.
		/// Only relative path from Assets folder is required, .png is automatically added.
		/// </summary>
		/// <param name="textures">Texture set containing names (paths) of .png files inside Assets directory without .png</param>
		public void setAnimation(string[] textures)
		{
			this.textures = textures;
			this.bitmaps = new CanvasBitmap[textures.GetLength(0)];
			Frame = 0;
		}

		/// <summary>
		/// Draws a texture on the screen using the current X and Y coordinates.
		/// Increments Frame counters.
		/// Supports lazy initialization of textures: if they werent loaded before, they will get loaded now.
		/// </summary>
		public void Draw()
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

			//Lazy initialization
			if (this.bitmaps == null || this.bitmaps[Frame] == null)
			{
				Log.i("Texture " + TextureToString(this.textures[Frame]) + " was not pre-loaded.");

				CreateResourcesAsync().GetAwaiter().GetResult();

				//var task = Task.Run(async () => { await CreateResourcesAsync() } ).Wait();
			}

			//Draw operation delegation to the Canvas Event Args
			DrawingSession.DrawImage(this.bitmaps[Frame], new Vector2(this.X, this.Y));

			//Updates current variables
			var size = this.bitmaps[Frame].Size;
			Width = size.Width;
			Height = size.Height;

			DrawHook();
		}

		//Post-Draw hook
		public virtual void DrawHook()
		{

		}

		//public void DrawOnCanvasAnimated(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
		//{
		//	this.DrawingSession = args.DrawingSession;
		//	this.CanvasControlSender = sender;
		//	Draw();
		//}

		private string TextureToString(string texture)
		{
			Log.i("converting texture name " + texture);
			return "Assets/" + texture + ".png";
		}

		//Loads all bitmap resources (frames) asynchronously
		public async Task CreateResourcesAsync()
		{
			LinkedList<IAsyncOperation<CanvasBitmap>> loadTasks = new LinkedList<IAsyncOperation<CanvasBitmap>>();
			LinkedList<int> loadTaskNumbers = new LinkedList<int>();

			for (int frame = 0; frame < this.textures.GetLength(0); frame++)
			{
				Log.i("Preparing frame " + frame.ToString());
				if (this.bitmaps[frame] == null)
				{
					loadTasks.AddLast(CanvasBitmap.LoadAsync(CanvasControlSender, TextureToString(this.textures[frame])));
					loadTaskNumbers.AddLast(frame);
				}
				Log.i("Prepared frame " + frame.ToString());
			}

			//Iterate over both lists and await all tasks
			LinkedListNode<IAsyncOperation<CanvasBitmap>> loadTask = loadTasks.First;
			LinkedListNode<int> loadTaskNumber = loadTaskNumbers.First;
			while (loadTask != null && loadTaskNumber != null)
			{
				this.bitmaps[loadTaskNumber.Value] = await (loadTask.Value);
				Log.i("Awaiting frame");

				loadTask = loadTask.Next;
				loadTaskNumber = loadTaskNumber.Next;
			}
		}

		//public async Task CreateResourcesAsync(ICanvasAnimatedControl sender)
		//{
		//	this.CanvasControlSender = sender;
		//	await CreateResourcesAsync();
		//}
	}
}
