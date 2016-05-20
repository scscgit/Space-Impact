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
using Windows.UI.ViewManagement;
using Space_Impact.Core.Game;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

/*
Saved code fragments:
var bounds = Window.Current.Bounds
TimeSpan.FromMilliseconds
*/

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

		IPlayer player = null;
		public IPlayer Player
		{
			get
			{
				return player;
			}
			private set
			{
				if(player != null)
				{
					RemoveActor(player);
				}
				player = value;
				AddActor(player);
			}
		}

		bool FirstDraw = true;

		//List of all actors that will receive Draw and Act callbacks
		LinkedList<IActor> ActorList;

		//Initialization of the class, before any textures are loaded
		public GameRound()
		{
			Log.i(this, "Initializing");
			this.InitializeComponent();

			ApplicationView.PreferredLaunchViewSize = new Size(1600, 900);
			//ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
			ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

			ActorList = new LinkedList<IActor>();
			Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
			Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;

			Log.i(this, "Initialized");
		}

		async void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
		{
			args.Handled = true;
			//await was not used in documentation
			await FieldControl.RunOnGameLoopThreadAsync(() => KeyUp_GameLoopThread(args.VirtualKey));
		}

		async void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
		{
			args.Handled = true;
			//await was not used in documentation
			await FieldControl.RunOnGameLoopThreadAsync(() => KeyDown_GameLoopThread(args.VirtualKey));
		}

		void KeyDown_GameLoopThread(VirtualKey virtualKey)
		{
			//const SpaceDirection.HorizontalDirection left = SpaceDirection.HorizontalDirection.LEFT;
			//const SpaceDirection.HorizontalDirection right = SpaceDirection.HorizontalDirection.RIGHT;
			//const SpaceDirection.VerticalDirection up = SpaceDirection.VerticalDirection.UP;
			//const SpaceDirection.VerticalDirection down = SpaceDirection.VerticalDirection.DOWN;

			//Movement keys
			switch (virtualKey)
			{
				case VirtualKey.A:
				case VirtualKey.Left:
					Player.Direction += SpaceDirection.HorizontalDirection.LEFT;
					break;
				case VirtualKey.D:
				case VirtualKey.Right:
					Player.Direction += SpaceDirection.HorizontalDirection.RIGHT;
					break;
				case VirtualKey.W:
				case VirtualKey.Up:
					Player.Direction += SpaceDirection.VerticalDirection.UP;
					break;
				case VirtualKey.S:
				case VirtualKey.Down:
					Player.Direction += SpaceDirection.VerticalDirection.DOWN;
					break;

				//Debug
				case VirtualKey.L:
					Log.d(this, "New player added");
					AddActor(new Hero());
					break;

				//DPI modifiers
				case VirtualKey.I:
					Log.i(this, "DPI modified to 0.5");
					FieldControl.DpiScale = 0.5f;
					break;
				case VirtualKey.O:
					Log.i(this, "DPI modified to 1");
					FieldControl.DpiScale = 1f;
					break;
				case VirtualKey.P:
					Log.i(this, "DPI modified to 2");
					FieldControl.DpiScale = 2f;
					break;
			}
		}

		void KeyUp_GameLoopThread(VirtualKey virtualKey)
		{
			switch (virtualKey)
			{
				//Movement keys
				case VirtualKey.A:
				case VirtualKey.Left:
					Player.Direction -= SpaceDirection.HorizontalDirection.LEFT;
					break;
				case VirtualKey.D:
				case VirtualKey.Right:
					Player.Direction -= SpaceDirection.HorizontalDirection.RIGHT;
					break;
				case VirtualKey.W:
				case VirtualKey.Up:
					Player.Direction -= SpaceDirection.VerticalDirection.UP;
					break;
				case VirtualKey.S:
				case VirtualKey.Down:
					Player.Direction -= SpaceDirection.VerticalDirection.DOWN;
					break;
			}
		}

		void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			//Removing events
			Log.i(this, "Page is being unloaded, removing events and other associations");
			Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
			Window.Current.CoreWindow.KeyUp -= CoreWindow_KeyUp;

			//Cleaning up the Page to help the garbage collector
			canvas.RemoveFromVisualTree();
			canvas = null;
			Log.i(this, "Page was fully unloaded");
		}

		//First Draw cycle of a Field calls this method
		void OnFirstDraw()
		{
			Log.i(this, "First draw has started");

			//Loading players ad-hoc
			Player = new Hero();
		}

		void canvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
		{
			Log.i(this, "CreateResources started");

			//On losing device (the GPU one, not your cheap Windows Phone), resources gotta get reloaded
			if (args.Reason == CanvasCreateResourcesReason.NewDevice)
			{
				Log.i(this, "Hey a new device");
				//sender.Device.DeviceLost += Device_DeviceLost; //or maybe not
			}
			//If the reason is other, e.g. DPI change, we have to reload our textures too
			TextureSetLoader.DeleteInstance();

			//We create resources asynchronously
			Task createResourcesAsync = CreateResourcesAsync(sender);
			args.TrackAsyncAction(createResourcesAsync.AsAsyncAction());
			Log.i(this, "CreateResources started parallel task for creating textures");
		
			//Failed attempt at syncing async
			//args.GetTrackedAction().AsTask().GetAwaiter().GetResult();
			//Log.i(this, "CreateResources parallel task has finished");			

			Log.i(this, "CreateResources finished");
		}

		//Loads all resources asynchronously
		async Task CreateResourcesAsync(CanvasAnimatedControl sender)
		{
			await TextureSetLoader.Instance.CreateResourcesAsync(sender);
		}

		//Main game loop, should be fired 60 times per second
		void canvas_DrawAnimated(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
		{
			if(FirstDraw)
			{
				FirstDraw = false;
				OnFirstDraw();
			}

			//Exception during Act is fatal. We encapsulate it as plain text and throw it.
			try
			{
				//Iterate over list and act on all actors
				LinkedListNode<IActor> actor = ActorList.First;
				while (actor != null)
				{
					//We allow the actor to remove himself from the list by taking care of a possible concurrent list modification problem
					IActor currentActor = actor.Value;
					actor = actor.Next;
					currentActor.Act();
				}
			}
			catch(Exception e)
			{
				throw new Exception("Exception happened during Act on an Actor.\n"+e.ToString());
			}

			//No problem should happen, but this makes debugging easier, plus any error during a single frame Draw is irrelevant anyway
			try
			{
				//Draw all actors
				foreach (IActor actor in ActorList)
				{
					actor.Draw(args.DrawingSession);
				}
			}
			catch (Exception e)
			{
				Log.e(this, e.ToString());
			}

			// args.DrawingSession.DrawText("hello", new System.Numerics.Vector2(100, 50), Windows.UI.Colors.Aqua);
			//args.DrawingSession.DrawImage(this.playerImage, new Vector2(a, 5));
		}

		void canvasTouchpad_Draw(CanvasControl sender, CanvasDrawEventArgs args)
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
			Log.i(this, "Adding actor " + actor.Name);
			if (actor != null)
			{
				actor.AddedToField(this);
				ActorList.AddLast(actor);
			}
		}
		public void RemoveActor(IActor actor)
		{
			Log.i(this, "Removing actor " + actor.Name);
			ActorList.Remove(actor);
		}

		//Button for opening left menu
		void MenuButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "User clicked on Hamburger");
			SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
		}

		void NewGameButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "User clicked on New Game Button");
			//todo

		}

		async void ExitGameButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "User clicked on Exit Game Button");
			var dialog = new Windows.UI.Popups.MessageDialog("Are you sure you don't want to keep playing?");

			dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes, I am") { Id = 0 });
			dialog.Commands.Add(new Windows.UI.Popups.UICommand("No, let me play") { Id = 1 });

			if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
			{
				//Adding a 3rd command will crash the app when running on Mobile !!!
				//dialog.Commands.Add(new Windows.UI.Popups.UICommand("Maybe later") { Id = 2 });
			}

			dialog.DefaultCommandIndex = 0;
			dialog.CancelCommandIndex = 1;

			switch ((int)(await dialog.ShowAsync()).Id)
			{
				case 0:
					//User decided to exit the game
					Log.i(this, "User has confirmed his choice to end the game via Exit Button");
					Window.Current.CoreWindow.Close();
					break;
				case 1:
				default:
					Log.i(this, "User has cancelled his choice to end the game via Exit Button");
					//No action when user decides to keep playing
					break;
			}
		}
	}
}
