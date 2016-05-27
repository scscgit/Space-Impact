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
using Space_Impact.Core.Graphics.Background.Strategy;
using Space_Impact.Core.Game.Spawner.Wrapper;
using Space_Impact.Core.Game.Level;
using Space_Impact.Core.Game.Character.Enemy;
using Space_Impact.Services;

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
		const string PageMusic = "core";

		/// <summary>
		/// Background class of the Field
		/// </summary>
		internal class GameRoundBackground : AbstractBackground
		{
			public GameRoundBackground(IField field) : base(field)
			{
				AddStrategy(new MovingUp(this, field));
				Animation = TextureSetLoader.BG1;
			}
		}

		IBackground BackgroundImage;
		MediaElement Music;

		//Represents if the screen is loaded (fully initialized) with all resources and can be safely navigated out from.
		bool fieldLoaded = false;
		public bool FieldLoaded
		{
			get
			{
				return this.fieldLoaded;
			}
			private set
			{
				this.fieldLoaded = value;
				if (this.fieldLoaded)
				{
					GameRoundLoadingGrid.Visibility = Visibility.Collapsed;
					GameRoundGrid.Visibility = Visibility.Visible;
					Log.i(this, "FieldLoaded set to true");
				}
				else
				{
					GameRoundLoadingGrid.Visibility = Visibility.Visible;
					GameRoundGrid.Visibility = Visibility.Collapsed;
					Log.i(this, "FieldLoaded set to false");
				}
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

		//Screen debug logging
		int LastLogCounter { get; set; } = 0;
		int LastLogCountTime { get; set; } = 0;
		int LastLogCountLimit { get; set; } = 20;

		//Message for user
		public int MessageBroadcastCounter { get; set; } = 400;
		public int MessageBroadcastTime { get; set; } = 400;
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
		/// Delegates the getter to the Field's percentage completion.
		/// </summary>
		public float Percent
		{
			get
			{
				return MovingUp.PercentageCompletion(BackgroundImage, this);
			}
		}

		string LastLog = "";

		bool FirstDraw = true;

		//List of all actors that will receive Draw and Act callbacks
		LinkedList<IAct> ActorList;
		//Lock for concurrent operations on the same ActorList
		private object ActorListLock = new object();

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
			Window.Current.SizeChanged -= Current_SizeChanged;
		}

		//Initialization of the class, before any textures are loaded
		public GameRound()
		{
			Log.i(this, "Constructor initializing");

			this.InitializeComponent();

			//Initializing loading screen
			FieldLoaded = false;

			RegisterEvents();

			ActorList = new LinkedList<IAct>();

			Log.i(this, "Constructor initialized");
		}

		//Page destructor
		void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			//Removing events
			Log.i(this, "Page is being unloaded, removing events and other associations");
			RemoveEvents();
			Music.Stop();

			//Cleaning up the Page to help the garbage collector
			canvas.RemoveFromVisualTree();
			canvas = null;
			Log.i(this, "Page was fully unloaded");
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
			if (!FieldLoaded)
			{
				return;
			}

			//The event is handled
			args.Handled = true;

			//await was not used in documentation
			await KeyUp_GameLoopThread(args.VirtualKey);
			//await FieldControl.RunOnGameLoopThreadAsync(() => KeyUp_GameLoopThread(args.VirtualKey));
		}

		async void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
		{
			//General condition for all key events is that the field is loaded
			if (!FieldLoaded)
			{
				return;
			}

			//The event is handled
			args.Handled = true;

			//await was not used in documentation
			await KeyDown_GameLoopThread(args.VirtualKey);
			//await FieldControl.RunOnGameLoopThreadAsync(() => KeyDown_GameLoopThread(args.VirtualKey)); //Problem s volanim ShowExitDialog() z herneho vlakna
		}

		async Task KeyDown_GameLoopThread(VirtualKey virtualKey)
		{
			//const SpaceDirection.HorizontalDirection left = SpaceDirection.HorizontalDirection.LEFT;
			//const SpaceDirection.HorizontalDirection right = SpaceDirection.HorizontalDirection.RIGHT;
			//const SpaceDirection.VerticalDirection up = SpaceDirection.VerticalDirection.UP;
			//const SpaceDirection.VerticalDirection down = SpaceDirection.VerticalDirection.DOWN;

			if (Player == null || !GameRunning)
			{
				Log.i(this, "There is no Player, or the game is not running and yet, KeyDown event was run");
			}
			else
			{
				//Player control keys
				switch (virtualKey)
				{
					//Movement keys
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
				}
			}

			//Game keys
			switch (virtualKey)
			{
				//Game lifecycle alteration
				case VirtualKey.Escape:
					Log.i(this, "Escape button pressed, opening dialog asynchronously");
					await ShowExitDialog();
					break;

				//Debug
				case VirtualKey.H:
					Player.Health = Player.MaxHealth;
					break;
				case VirtualKey.J:
					Music.Play();
					break;
				case VirtualKey.K:
					Music.Pause();
					break;
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
			if (Player == null || !GameRunning)
			{
				Log.i(this, "There is no Player, or the game is not running and yet, KeyUp event was run");
			}
			else
			{
				//Player control keys
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

			//Game keys
			/*switch (virtualKey)
			{

			}*/
		}

		//First Draw cycle of a Field calls this method
		void BeforeFirstDraw()
		{
			Log.i(this, "BeforeFirstDraw has started");

			//Instance of a background image
			Log.i(this, "Creating instance of background image");
			BackgroundImage = new GameRoundBackground(this);

			//Loading player ad-hoc
			Player = new Hero();
			Player.X = (float)Size.Width / 2 - (float)Player.Width / 2;
			Player.Y = (float)Size.Height / 2 - (float)Player.Height / 2;

			//Creating a Level
			new Level1().AddToField(this);

			//Also objects ad-hoc
			//ICharacter doomday = new Doomday(Player);
			//doomday.X = 600;
			//doomday.Y = 400;
			//AddActor(doomday);

			//The game-flow starts running after the map is loaded with all characters
			GameRunning = true;

			//Resetting user input for avoiding possible start glitches that would require user to alt+tab for a hotfix instead
			ResetUserInput();

			Log.i(this, "BeforeFirstDraw has ended");
		}

		void AfterFirstDraw()
		{
			Log.i(this, "AfterFirstDraw has started");

			//todo this would be ideal place for switching texture loading screen, BUT it is in a different thread!!!

			Log.i(this, "AfterFirstDraw has ended");
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
			Log.i(this, "CreateResources starting parallel task for creating textures");
			args.TrackAsyncAction(createResourcesAsync.AsAsyncAction());

			//Failed attempt at syncing async
			//args.GetTrackedAction().AsTask().GetAwaiter().GetResult();
			//Log.i(this, "CreateResources parallel task has finished");			

			Log.i(this, "CreateResources finished");
		}

		//Loads all resources asynchronously
		async Task CreateResourcesAsync(CanvasAnimatedControl sender)
		{
			//Increases progress bar percentage
			await TextureSetLoader.Instance.CreateResourcesAsync
			(
				sender
				//Increases progress bar percentage during loading
				, (increasePercentage) => { loadingProgressBar.Value += increasePercentage; }
				//If not null, gets called back after loading gets finished
				, null
				//Loads all textures implicitly
				);

			//Music is also a resource
			Log.i(this, "Loading music");
			Music = await Utility.GetMusic(PageMusic);

			//Music is implicitly started, but to be sure, we explicitly start it
			//BUG: Music is not being looped
			Music.Loaded += (a, b) => { Music.IsLooping = true; };
			Music.IsLooping = true;
			Music.Play();
			Music.IsLooping = true;

			Log.i(this, "Setting Field as loaded");
			FieldLoaded = true;

			Log.i(this, "CreateResourcesAsync finished");
		}

		//Main game loop, should be fired 60 times per second
		void canvas_DrawAnimated(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
		{
			//Calling actions expected before first draw starts
			if (FirstDraw)
			{
				BeforeFirstDraw();
			}

			//Exception during Act is fatal. We encapsulate it as a plain text and throw it.
			IAct problemActor = null;
			try
			{
				ForEachActor<IAct>
				(
					a =>
					{
						problemActor = a;
						a.Act();
						return false;
					}
				);
			}
			catch (Exception e)
			{
				Log.e(this, "Exception happened during Act on an Actor.\n" + e.ToString());
				if (problemActor != null)
				{
					Log.e(this, "Problem Actor was " + problemActor.ToString());
				}
				//sender.RunOnGameLoopThreadAsync(async() => { await Utility.GetMusic("Sounds", "alarm_to_the_extreme"); }).AsTask();
				//throw new Exception("Exception happened during Act on an Actor.\n" + e.ToString(), e);
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

			//Draws current Score
			{
				var format = new Microsoft.Graphics.Canvas.Text.CanvasTextFormat();
				format.FontSize = 30;
				format.FontFamily = "Arial";
				args.DrawingSession.DrawText("Score: " + PlayerController.SumScore(), new Vector2(15, 15), Colors.ForestGreen, format);
			}

			//Calling actions expected after first draw finishes
			if (FirstDraw)
			{
				FirstDraw = false;
				AfterFirstDraw();
			}
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
			if (actor != null)
			{
				if (actor is IPlacedInField)
				{
					((IPlacedInField)actor).AddedToField(this);
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
			ActorList.Remove(actor);
		}

		//Performs an operation on each actor on the Field.
		//If the operation returns true, iteration MAY stop (depending on implementation, the value is ignored for now).
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
		}

		async void NewGameButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "User clicked on New Game Button");
			string message = "Are you sure you want to start a new game?";
			if (GameRunning)
			{
				message += "\nYou will lose all of the current progress!";
			}
			await PopupDialog.ShowPopupDialog(message, "Yes, I am", () => ChangeScreen(typeof(GameRound)), "No, let me play", null);
		}

		async void ExitGameButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "User clicked on Exit Game Button");
			await ShowExitDialog();
		}

		//Exit Dialog implementation
		async Task ShowExitDialog()
		{
			await PopupDialog.ShowPopupDialog("Are you sure you don't want to keep playing?", "Yes, I am", () => ChangeScreen(typeof(MainMenu)), "No, let me play", null);
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

		/// <summary>
		/// Game has ended, by winning or losing.
		/// </summary>
		public void GameOver()
		{
			GameRunning = false;
			ResetUserInput();

			//Kills all enemies
			ForEachActor<IEnemy>
			(
				enemy =>
				{
					enemy.Health = 0;
					return false;
				}
			);

			//Removes all spawners
			ForEachActor<ISpawner>
			(
				spawner =>
				{
					spawner.DeleteActor();
					return false;
				}
			);

			//Implicitly assumes the player has won, so he starts moving upwards victoriously
			Player.Direction = SpaceDirection.Get(SpaceDirection.HorizontalDirection.NONE, SpaceDirection.VerticalDirection.UP);
		}

		//The exit point of the page
		void ChangeScreen(Type screenType)
		{
			//Exiting before loading resources would cause synchronization hazard
			if (!FieldLoaded)
			{
				return;
			}

			//The literally only way I could find to prevent music from loading and running even after it was stopped before old Page disposal
			if (Music.CurrentState == MediaElementState.Opening)
			{
				return;
			}

			//GameRunning set to false just in case to be sure no parallel task will interrupt the process
			//GameRunning = false;

			//TODO FIX NAVIGATION, IT CRASHES BECAUSE OF PARALLEL PROCESSES
			Frame.Navigate(screenType);
		}
	}
}
