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
using Space_Impact.Core.Game.Object;
using Space_Impact.Core.Game.Character;
using Space_Impact.Core.Game.Spawner;
using Space_Impact.Core.Graphics.Background;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

/*
Saved code fragments:
var bounds = Window.Current.Bounds
TimeSpan.FromMilliseconds
*/

namespace Space_Impact.Screen
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class GameRound : Page, IField
	{
		/// <summary>
		/// Background class of the Field
		/// </summary>
		internal class GameRoundBackground : AbstractBackground
		{
			public GameRoundBackground(IField field): base(field)
			{
				Animation = TextureSetLoader.BG1;
			}
		}

		//The game should be running only after the map is loaded with all characters and no null problems can happen
		public bool GameRunning
		{
			get; private set;
		} = false;

		//Accessor point of the Field Canvas Animated Control
		public CanvasAnimatedControl FieldControl
		{
			get
			{
				return this.canvas;
			}
		}

		//Current player on the Field
		IPlayer player = null;
		public IPlayer Player
		{
			get
			{
				return player;
			}
			private set
			{
				if (player != null)
				{
					RemoveActor(player);
				}
				player = value;
				AddActor(player);
			}
		}

		//Current size of the Field
		public Size Size
		{
			get
			{
				return this.canvas.Size;
			}
		}

		IBackground BackgroundImage
		{
			get; set;
		}

		//Screen debug logging
		int LastLogCounter { get; set; } = 0;
		int LastLogCountTime { get; set; } = 0;
		int LastLogCountLimit { get; set; } = 20;

		//Message for user
		public int MessageBroadcastCounter { get; set; } = 220;
		public int MessageBroadcastTime { get; set; } = 220;
		string messageBroadcastText = "";
		public string MessageBroadcastText
		{
			get
			{
				return this.messageBroadcastText;
			}

			set
			{
				this.messageBroadcastText = value;
				MessageBroadcastCounter = 0;
			}
		}

		/// <summary>
		/// Delegates the getter to the Field's percentage completion
		/// </summary>
		public float Percent
		{
			get
			{
				return BackgroundImage.Percent;
			}
		}

		string LastLog = "";

		bool FirstDraw = true;

		//List of all actors that will receive Draw and Act callbacks
		LinkedList<IAct> ActorList;
		//Lock for concurrent operations on the same ActorList
		private object ActorListLock = new object();

		//Actions that happen after the end of an animated Draw cycle
		void AfterDraw()
		{
			//todo doesnt work delete but idk maybe do something with resolution
			if (Window.Current == null) return;
			Log.d(this, Window.Current.Bounds.Width + "x");
		}

		//Registering events
		void RegisterEvents()
		{
			//Keyboard presses
			Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
			Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;

			//Instead of losing/gaining focus (which doesn't work properly),
			//reinitialization of some aspects like KeyUp/KeyDown are best handled in Window Activated Event
			Window.Current.Activated += Current_Activated;
			//SizeChanged does cancel the focus without triggering Activation
			Window.Current.SizeChanged += Current_SizeChanged;
			//LostFocus += GameRound_LostFocus;
			//Window.Current.Content.LostFocus += Content_LostFocus;
		}

		//Unregistering events
		void RemoveEvents()
		{
			//Keyboard presses
			Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
			Window.Current.CoreWindow.KeyUp -= CoreWindow_KeyUp;

			//Window focus
			Window.Current.Activated -= Current_Activated;
		}

		//Initialization of the class, before any textures are loaded
		public GameRound()
		{
			Log.i(this, "Initializing");
			this.InitializeComponent();

			//Resolution of the game
			ApplicationView.PreferredLaunchViewSize = new Size(1600, 900);
			//ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
			ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

			RegisterEvents();

			ActorList = new LinkedList<IAct>();

			Log.i(this, "Initialized");
		}

		//Reinitialization of all basic user-controlled logic after losing track of inputs
		void ResetUserInput()
		{
			//Window.Current events can happen during game initialization
			if (Player != null)
			{
				Player.Direction = SpaceDirection.None;
				Player.Shooting = false;
			}
		}

		void Current_Activated(object sender, WindowActivatedEventArgs e)
		{
			Log.i(this, "Event Window.Current.Activated, resetting user-controlled (input) logic");
			ResetUserInput();
		}

		void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
		{
			Log.i(this, "Event Window.Current.SizeChanged, resetting user-controlled (input) logic");
			ResetUserInput();
		}

		async void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
		{
			if (!GameRunning)
			{
				return;
			}

			args.Handled = true;
			//await was not used in documentation

			await KeyUp_GameLoopThread(args.VirtualKey);
			//await FieldControl.RunOnGameLoopThreadAsync(() => KeyUp_GameLoopThread(args.VirtualKey));
		}

		async void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
		{
			if (!GameRunning)
			{
				return;
			}

			args.Handled = true;
			//await was not used in documentation

			await KeyDown_GameLoopThread(args.VirtualKey);
			//await FieldControl.RunOnGameLoopThreadAsync(() => KeyDown_GameLoopThread(args.VirtualKey)); //Problem s volanim ShowExitDialog() z herneho vlakne
		}

		async Task KeyDown_GameLoopThread(VirtualKey virtualKey)
		{
			//const SpaceDirection.HorizontalDirection left = SpaceDirection.HorizontalDirection.LEFT;
			//const SpaceDirection.HorizontalDirection right = SpaceDirection.HorizontalDirection.RIGHT;
			//const SpaceDirection.VerticalDirection up = SpaceDirection.VerticalDirection.UP;
			//const SpaceDirection.VerticalDirection down = SpaceDirection.VerticalDirection.DOWN;

			if (Player == null)
			{
				Log.e(this, "There is no Player and yet, KeyDown event was run");
			}

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

				//Shooting
				case VirtualKey.F:
				case VirtualKey.Space:
				case VirtualKey.LeftControl:
					if (Player.Shooting == false)
					{
						Log.i(this, "Player is shooting");
						Player.Shooting = true;
					}
					break;

				//Game lifecycle alteration
				case VirtualKey.Escape:
					Log.i(this, "Escape button pressed, opening dialog asynchronously");
					await ShowExitDialog();
					break;

				//Debug
				case VirtualKey.L:
					Log.d(this, "New player added");
					MessageBroadcastText = "Hello world";
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

		async Task KeyUp_GameLoopThread(VirtualKey virtualKey)
		{
			if (Player == null)
			{
				Log.e(this, "There is no Player and yet, KeyUp event was run");
			}

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

				//Shooting
				case VirtualKey.F:
				case VirtualKey.Space:
				case VirtualKey.LeftControl:
					if (Player.Shooting)
					{
						Player.Shooting = false;
					}
					break;
			}
		}

		void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			//Removing events
			Log.i(this, "Page is being unloaded, removing events and other associations");
			RemoveEvents();

			//Cleaning up the Page to help the garbage collector
			canvas.RemoveFromVisualTree();
			canvas = null;
			Log.i(this, "Page was fully unloaded");
		}

		//First Draw cycle of a Field calls this method
		void OnFirstDraw()
		{
			Log.i(this, "First draw has started");

			//Instance of a background image
			Log.i(this, "Creating instance of background image");
			BackgroundImage = new GameRoundBackground(this);

			//Loading player ad-hoc
			Player = new Hero();
			Player.X = (float)Size.Width / 2 - (float)Player.Width / 2;
			Player.Y = (float)Size.Height / 2 - (float)Player.Height / 2;

			//Initializing spawner
			ISpawner spawner = new EnemySpawner(this, 600, 400);
			AddActor(spawner);

			//Also objects adhoc
			//ICharacter doomday = new Doomday(Player);
			//doomday.X = 600;
			//doomday.Y = 400;
			//AddActor(doomday);

			//The game starts running after the map is loaded with all characters
			GameRunning = true;

			//Resetting user input for avoiding possible start glitches that would require user to alt+tab for a hotfix instead
			ResetUserInput();
		}

		void canvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
		{
			Log.i(this, "CreateResources started");

			//On losing device (the GPU one, not your cheap Windows Phone), resources gotta get reloaded
			if (args.Reason == CanvasCreateResourcesReason.NewDevice)
			{
				Log.i(this, "Hey, a new device!");
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
			if (FirstDraw)
			{
				FirstDraw = false;
				OnFirstDraw();
			}

			//Exception during Act is fatal. We encapsulate it as a plain text and throw it.
			try
			{
				ForEachActor<IAct>
				(
					a =>
					{
						a.Act();
						return false;
					}
				);
			}
			catch (Exception e)
			{
				throw new Exception("Exception happened during Act on an Actor.\n" + e.ToString(), e);
			}

			//Background draw cycle
			try
			{
				BackgroundImage.Draw(args.DrawingSession);
			}
			catch (Exception e)
			{
				Log.e(this, "Problem happened during Background Draw.\n" + e.ToString());
			}

			//Actor draw cycle
			//No problem should happen, but this makes debugging easier, plus any error during a single frame Draw is irrelevant anyway
			try
			{
				//Draw all actors
				ForEachActor<IActor>
				(
					a =>
					{
						a.Draw(args.DrawingSession);
						return false;
					}
				);

				//Draw last error log
				DrawErrorLog(args.DrawingSession);

			}
			catch (Exception e)
			{
				Log.e(this, "Problem happened during Draw.\n" + e.ToString());
			}

			//Draws a broadcast message, if there is any
			if (MessageBroadcastCounter < MessageBroadcastTime)
			{
				var format = new Microsoft.Graphics.Canvas.Text.CanvasTextFormat();
				format.FontSize = 120;
				format.FontStyle = Windows.UI.Text.FontStyle.Oblique;
				format.FontFamily = "Comic Sans";

				//If I ever implement multi-line messages, this calculation has to take the longest line instead of length
				int moveToLeft = MessageBroadcastText.Length * 5;
				args.DrawingSession.DrawText(MessageBroadcastText, new Vector2(500 - moveToLeft, 200), Colors.Aquamarine, format);
				MessageBroadcastCounter++;
			}

			AfterDraw();
		}

		void DrawErrorLog(CanvasDrawingSession drawingSession)
		{
			//Kym neskoncil odpocet, tak pocitame a cakame
			if (LastLogCounter < LastLogCountTime)
			{
				LastLogCounter++;
			}
			//Ked nastal cas na novu spravu, napise sa
			else
			{
				//Vytiahne si z logu dalsiu spravu
				string message = Log.NextMessage;

				//Ak prisla nova sprava, stara sa nou nahradi a zacne odpocet odznovu
				if (message != null)
				{
					LastLog = message;
					LastLogCounter = 0;

					//Dalsi odpocet bude trvat kratsie z dovodu rychlejsieho vypisu velkeho poctu hodnot
					if (LastLogCountTime > 1)
					{
						LastLogCountTime = (int)(LastLogCountTime * 0.85);
					}
				}
				//Ak neprisla nova sprava, tak zacneme spomalovat interval medzi vypismi pre lepsiu prehladnost
				else if (LastLogCountTime < LastLogCountLimit)
				{
					LastLogCountTime++;
				}
			}

			//Samotny vypis hlasky na obrazovku, cim dlhsi text, tym viac dolava sa musi posunut jeho zaciatok
			drawingSession.DrawText
				(
				LastLog,
				(float)Size.Width - 100 - 9.40f * LastLog.Length,
				(float)Size.Height - 50,
				LastLogCountTime == 1 ?
					Colors.Tomato
					:
					LastLogCountTime < LastLogCountLimit / 2 ?
						Colors.Indigo
						:
						Colors.DarkGreen
				);
		}

		//Manipulation with available actors that are currently registered using Observer pattern for receiving events
		public void AddActor(IAct actor)
		{
			Log.i(this, "Adding actor " + actor.ToString());
			if (actor != null)
			{
				if (actor is IActor)
				{
					((IActor)actor).AddedToField(this);
					ActorList.AddLast(actor);
				}
				else
				{
					ActorList.AddLast(actor);
				}
			}
		}
		public void RemoveActor(IAct actor)
		{
			Log.i(this, "Removing actor " + actor.ToString());
			ActorList.Remove(actor);
		}

		//Performs an operation on each actor on the Field.
		//If the operation returns true, iteration will stop.
		public void ForEachActor<ActorType>(ActorAction<ActorType> action)
		{
			//This operation is synchronized
			lock (ActorListLock)
			{
				//Iterate over the list and Act on all actors
				LinkedListNode<IAct> actor = ActorList.First;
				while (actor != null)
				{
					//We allow the actor to remove himself from the list by taking care of a possible concurrent list modification problem
					IAct currentActor = actor.Value;
					actor = actor.Next;

					//Only Actors with the correct type will act
					if (currentActor is ActorType)
					{
						//Action returns true if it intends to modify the list, but we don't use that value
						action((ActorType)currentActor);
					}
				}
			}
		}

		//Button for opening left menu
		void MenuButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "User clicked on the Hamburger");
			SplitView.IsPaneOpen = !SplitView.IsPaneOpen;

			//SplitView.Focus(FocusState.Unfocused);
		}

		void NewGameButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "User clicked on New Game Button");
			//todo

		}

		async void ExitGameButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "User clicked on Exit Game Button");
			await ShowExitDialog();
		}

		async Task ShowExitDialog()
		{
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
					ExitScreen();
					break;
				case 1:
				default:
					Log.i(this, "User has cancelled his choice to end the game via Exit Button");
					//No action when user decides to keep playing
					break;
			}
		}

		//Converts the position representing a point on a Window to a point on the Field
		Point WindowToFieldPosition(Point WindowPosition)
		{
			Point FieldPosition = new Point();
			var WindowBounds = Window.Current.CoreWindow.Bounds;
			FieldPosition.X = WindowPosition.X / WindowBounds.Width * Size.Width;
			FieldPosition.Y = WindowPosition.Y / WindowBounds.Height * Size.Height;
			return FieldPosition;
		}

		/// <summary>
		/// Pointer position capture event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e">Position is located in e.GetCurrentPoint(this).Position in Window format, conversion via WindowToFieldPosition() is advised</param>
		void Grid_PointerPressed(object sender, PointerRoutedEventArgs e)
		{
			Point point = WindowToFieldPosition(e.GetCurrentPoint(this).Position);

			//Clicks on all clickable actors
			ForEachActor<IClickable>
			(
				a =>
				{
					a.Click(point.ToVector2().X, point.ToVector2().Y);
					return false;
				}
			);
		}
		void Grid_PointerMoved(object sender, PointerRoutedEventArgs e)
		{
			Point point = WindowToFieldPosition(e.GetCurrentPoint(this).Position);

			//Event on all clickable actors
			ForEachActor<IClickable>
			(
				a =>
				{
					//Log.d(this, "Trying to ClickMove on IClickable actor " + actor.ToString() + "."); todo delete, just testing for a certain type of crash
					a.ClickMove(point.ToVector2().X, point.ToVector2().Y);
					return false;
				}
			);
		}
		void Grid_PointerReleased(object sender, PointerRoutedEventArgs e)
		{
			//Event on all clickable actors
			ForEachActor<IClickable>
			(
				a =>
				{
					a.ClickRelease();
					return false;
				}
			);
		}

		void SplitView_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
		{
			Log.i(this, "Closing panel, overriding focus");

			//Takes focus away from the Panel
			this.Focus(FocusState.Keyboard);
		}

		public void GameOver()
		{
			//Display a message and stop the gameflow
			MessageBroadcastText = "Game Over,\nYou've lost!";
			GameRunning = false;
			ResetUserInput();

			//todo pause and then get back to previous screen, TODO HOW TO PREVIOUS SCREEN (will use ExitScreen method)
		}

		void ExitScreen()
		{
			//Version that crashed the application with exception: Window.Current.CoreWindow.Close();
			Application.Current.Exit();
		}
	}
}
