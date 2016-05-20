using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas;
using System.Threading.Tasks;
using System.Numerics;
using Windows.UI;

using Windows.UI.Core;
using Windows.UI.Input;

using Space_Impact.Core.Game.Player;
using Space_Impact.Core;
using Space_Impact.Graphics;
using Space_Impact.Support;
using Windows.System;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Space_Impact.src
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class GameRound : Page, IField
	{
		//Accessor point of the Field Canvas Animated Control
		public CanvasAnimatedControl FieldControl
		{
			get
			{
				return this.canvas;
			}
		}

		//List of all actors that will receive Draw and Act callbacks
		LinkedList<IActor> ActorList;

		//Initialization
		public GameRound()
		{
			this.InitializeComponent();
			ActorList = new LinkedList<IActor>();

			Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

			//AddActor(new MovementThrust());
			//AddActor(new Hero());
		}

		private async void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
		{
			args.Handled = true;
			//await was not used in documentation
			await FieldControl.RunOnGameLoopThreadAsync(() => KeyDown_GameLoopThread(args.VirtualKey));
		}

		private void KeyDown_GameLoopThread(VirtualKey virtualKey)
		{
			switch (virtualKey)
			{
				case VirtualKey.A:
				case VirtualKey.Left:
					AddActor(new Hero());
					break;
				case VirtualKey.D:
				case VirtualKey.Right:
					Log.i("R");
					break;
				case VirtualKey.W:
				case VirtualKey.Up:

					break;
				case VirtualKey.S:
				case VirtualKey.Down:

					break;
			}
		}

		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			canvas.RemoveFromVisualTree();
			canvas = null;
		}

		private void canvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
		{
			//We create resources asynchronously
			args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
		}

		//Loads all resources asynchronously
		async Task CreateResourcesAsync(CanvasAnimatedControl sender)
		{
			await TextureSetLoader.Instance.CreateResourcesAsync(sender);
		}

		//Main game loop, should be fired 60 times per second
		private void canvas_DrawAnimated(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
		{
			{
				//Iterate over list and act on actors
				LinkedListNode<IActor> actor = ActorList.First;
				while (actor != null)
				{
					actor.Value.Act();
					actor = actor.Next;
				}
			}

			{
				//Draw all actors
				foreach (IActor actor in ActorList)
				{
					actor.Draw(args.DrawingSession);
				}
			}

			// args.DrawingSession.DrawText("hello", new System.Numerics.Vector2(100, 50), Windows.UI.Colors.Aqua);
			//args.DrawingSession.DrawImage(this.playerImage, new Vector2(a, 5));
		}

		private void canvasTouchpad_Draw(CanvasControl sender, CanvasDrawEventArgs args)
		{
			//args.DrawingSession.DrawCircle(new Vector2(10, 10), 5, Colors.Cyan);
		}

		/// <summary>
		/// Pointer capture event
		/// </summary>
		//private void canvasTouchpad_PointerPressed(object sender, PointerRoutedEventArgs e)
		//{
		//	Pointer pointer = e.Pointer;
		//	if (pointer.IsInContact)
		//	{
		//		canvas.Foreground = new SolidColorBrush(Colors.AliceBlue);
		//	}
		//	else
		//	{
		//		canvas.Foreground = new SolidColorBrush(Colors.Black);
		//	}
		//}

		//Manipulation with available actors that are currently registered using Observer pattern for receiving events
		public void AddActor(IActor actor)
		{
			if (actor != null)
			{
				actor.AddedToField(this);
				ActorList.AddLast(actor);
			}
		}
		public void RemoveActor(IActor actor)
		{
			ActorList.Remove(actor);
		}
	}
}
