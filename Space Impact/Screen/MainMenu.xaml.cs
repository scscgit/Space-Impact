using Space_Impact.Core;
using Space_Impact.Core.Graphics.Background;
using Space_Impact.Graphics;
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
using System.Threading.Tasks;
using Space_Impact.Core.Game;
using Space_Impact.Core.Game.Player;
using Space_Impact.Core.Graphics.Background.Strategy;
using Space_Impact.Support;
using Windows.UI.ViewManagement;
using Space_Impact.Services;
using Microsoft.EntityFrameworkCore;
using Windows.ApplicationModel.Background;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Space_Impact.Screen
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainMenu : Page, IField
	{
		const string PageMusic = "userspace"; //"vault";
		private const double DEFAULT_SOUND_VOLUME = 80;

		/// <summary>
		/// Background class of the MainMenu.
		/// </summary>
		internal class MainMenuBackground : AbstractBackground
		{
			public static readonly string[][] backgrounds = new string[][]
			{
				TextureSetLoader.BG_CRATER_PLANET, TextureSetLoader.BG_MESSIER,
				TextureSetLoader.BG_ROCKET_AND_PLANET, TextureSetLoader.BG_STAR_CLUSTERS
			};

			public MainMenuBackground(IField field) : base(field)
			{
				Animation = backgrounds[Utility.RandomBetween(0, backgrounds.Length - 1)];
				Speed = 5;
				AddStrategy(new RandomMovement(this, field));
			}
		}

		IBackground BackgroundImage;
		MediaElement Music;
		BackgroundTaskStatus BackgroundTaskStatus;

		//Represents if the screen is loaded (fully initialized) with all resources and can be safely navigated out from.
		//The Settings Grid gets implicitly disabled each time the Field Loaded state changes.
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
					MainMenuLoadingGrid.Visibility = Visibility.Collapsed;
					MainMenuGrid.Visibility = Visibility.Visible;
					Log.i(this, "FieldLoaded set to true");
				}
				else
				{
					MainMenuLoadingGrid.Visibility = Visibility.Visible;
					MainMenuGrid.Visibility = Visibility.Collapsed;
					Log.i(this, "FieldLoaded set to false");
				}
			}
		}

		public bool GameRunning
		{
			get; private set;
		} = false;

		public CanvasAnimatedControl FieldControl
		{
			get
			{
				return this.canvas;
			}
		}

		public IPlayer Player
		{
			get
			{
				return null;
			}
		}

		public Size Size
		{
			get
			{
				return FieldControl.Size;
			}
		}

		public string MessageBroadcastText
		{
			get; set;
		}

		public float Percent
		{
			get
			{
				return 0;
			}
		}

		bool FirstDraw = true;

		//Initialization of the class, before any textures are loaded
		public MainMenu()
		{
			Log.i(this, "Constructor initializing");

			this.InitializeComponent();

			//Initializing loading screen
			FieldLoaded = false;

			//Preparing Main Screen of MainMenu grid
			MainMenuMainScreenGrid.Visibility = Visibility.Visible;
			MainMenuSettingsGrid.Visibility = Visibility.Collapsed;

			//Loading screen resolution settings
			LoadScreenResolution();

			//Loading sound volume settings
			LoadSoundVolume();

			//Initializing Database of Players and loading them
			InitializeAndLoadPlayers();

			//Loading debug settings
			LoadDebugSetting();

			Log.i(this, "Constructor initialized");
		}

		//Page destructor
		void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			Log.i(this, "Page is being unloaded, removing events and other associations");
			Log.i(this, "Music state before stopping was " + Music.CurrentState.ToString());

			Music.Stop();

			//Cleaning up the Page to help the garbage collector
			canvas.RemoveFromVisualTree();
			canvas = null;

			Log.i(this, "Page was fully unloaded");
		}

		/// <summary>
		/// Initializes screen resolution view size from settings.
		/// Loads default values when the settings are not found, width and height are reset both at once to prevent user mistakes.
		/// Updates corresponding values in the Settings sub-menu.
		/// Order:
		/// First the fullscreen is evaluated so that if it changes, resolution update can follow and will not be ignored.
		/// After that, width and height sizes get set accordingly.
		/// </summary>
		void LoadScreenResolution()
		{
			//Default fullscreen value is false
			bool resolutionFullscreen = Utility.SettingsLoad<bool>("resolutionFullscreen");

			//Resolution of the game screen
			int resolutionWidth;
			int resolutionHeight;
			string resolutionWidthString = Utility.SettingsLoad<string>("resolutionWidth");
			string resolutionHeightString = Utility.SettingsLoad<string>("resolutionHeight");

			//Parsing values from settings, setting both to default on error
			bool resultWidth = int.TryParse(resolutionWidthString, out resolutionWidth);
			bool resultHeight = int.TryParse(resolutionHeightString, out resolutionHeight);

			//Default values
			if (!resultWidth || !resultHeight)
			{
				resolutionWidth = 1600;
				resolutionHeight = 900;
			}

			Log.i(this, "Loading fullscreen choice from settings: " + (resolutionFullscreen ? "true" : "false"));
			Log.i(this, "Loading width resolution from settings: " + (resultWidth ? "success" : "failure") + ", used " + resolutionWidth.ToString());
			Log.i(this, "Loading height resolution from settings: " + (resultHeight ? "success" : "failure") + ", used " + resolutionHeight.ToString());

			//Applying the values to the current window
			Size windowSize = new Size(resolutionWidth, resolutionHeight);
			var view = ApplicationView.GetForCurrentView();

			//Force-resize on top of the preferred setting in case the function is called again (after settings get changed)
			if (resolutionFullscreen)
			{
				ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
				view.TryEnterFullScreenMode();
			}
			else
			{
				ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
				view.ExitFullScreenMode();
			}
			ApplicationView.PreferredLaunchViewSize = windowSize;
			view.TryResizeView(windowSize);

			//Updating values in the Settings submenu
			settingsResolutionWidth.Text = resolutionWidth.ToString();
			settingsResolutionHeight.Text = resolutionHeight.ToString();
			settingsFullscreen.IsChecked = resolutionFullscreen;
		}

		void LoadSoundVolume()
		{
			double? resultVolume = Utility.SettingsLoad<double?>("resultVolume");

			Log.i(this, "Loading sound volume choice from settings: " + (resultVolume == null ? ("failure, defaulting to " + DEFAULT_SOUND_VOLUME) : resultVolume.ToString()));

			//Default value
			if (resultVolume == null)
			{
				resultVolume = DEFAULT_SOUND_VOLUME;
			}

			//Registers the Value Changed event to modify Music volume in real time
			settingsVolume.ValueChanged += (a, b) => LoadVolume();
			settingsVolume.Value = resultVolume.Value;
		}

		void LoadVolume()
		{
			if (Music != null)
			{
				Music.Volume = settingsVolume.Value;
			}
		}

		void InitializeAndLoadPlayers()
		{
			using (var db = new Persistence())
			{
				//Creates the Database if it does not exist
				db.Database.EnsureCreated();

				var players = db.Players;

				//Displays all Players
				UpdatePlayers(db.Players);

				//Selects last selected Player
				int? selectedPlayerId = Utility.SettingsLoad<int?>("selectedPlayer");
				if (selectedPlayerId != null)
				{
					//If we used the lambda expression to get the first player without making sure there is one, game could crash
					//We fix the problem before announcing the error by erasing the saved ID
					if (db.Players.Count(p => p.Id == selectedPlayerId) != 1)
					{
						Utility.SettingsSave<int?>("selectedPlayer", null);
						Log.e(this, "Wrong number of players with a requested ID! There are in total " + db.Players.Count().ToString() + " players in DB. Deleting setting choice.");
						return;
					}

					//Instance of the last selected Player
					Services.Entity.Player player = db.Players.Where(p => p.Id == selectedPlayerId).First();

					//This is not expected to happen, so we log an error and fix the problem
					if (player == null)
					{
						Utility.SettingsSave<int?>("selectedPlayer", null);
						Log.e(this, "Player selection restored, but there was no such player in database. Deleting setting choice.");
						return;
					}

					//Selects a Player for the first time
					PlayersComboBox.SelectedItem = player;
					Log.i(this, "Restored player selection choice to " + player.Name);
				}
				else
				{
					//By default, the Main Menu is in state of no Player being selected
					Log.i(this, "There was no saved ID for selected Player");
				}
			}
			Log.i(this, "Players initialized and loaded");
		}

		/// <summary>
		/// Updates all structures that work with Players.
		/// Should be called when the Database state changes.
		/// </summary>
		/// <param name="players">current list of players in the Database</param>
		void UpdatePlayers(DbSet<Services.Entity.Player> players)
		{
			if (players == null)
			{
				Log.e(this, "Players in Database returned null");
				return;
			}

			//Updates list of Players that can be chosen from
			PlayersComboBox.ItemsSource = players.ToList();
		}

		/// <summary>
		/// Implicit (less efficient) overload of updating Players.
		/// Downloads a current up-to-date version from the Database.
		/// </summary>
		void UpdatePlayers()
		{
			DbSet<Services.Entity.Player> players;
			using (var db = new Persistence())
			{
				players = db.Players;
			}
			UpdatePlayers(players);
		}

		/// <summary>
		/// Called after a new CheckBox choice of a current Player.
		/// Takes care of persisting the choice.
		/// Also enables or disables the New Game button.
		/// </summary>
		/// <param name="player">new current Player</param>
		void AfterSelectedPlayer(Services.Entity.Player player)
		{
			//Storing the instance of the selected Player to the Singleton controller
			PlayerController.Player = player;

			//If the player is not valid, overwrites the stored selected player value by null, otherwise saves him
			//Enables or disables the New Game button
			if (player == null)
			{
				Utility.SettingsSave<int?>("selectedPlayer", null);
				newGameButton.Style = (Style)Resources["NewGameButtonDisabled"];
				Log.i(this, "Deleted player selection choice");
			}
			else
			{
				Utility.SettingsSave<int?>("selectedPlayer", player.Id);
				newGameButton.Style = (Style)Resources["NewGameButtonEnabled"];
				Log.i(this, "Saved player selection choice");
			}
		}

		/// <summary>
		/// Initializes the Debugging CheckBox to the current saved value.
		/// </summary>
		void LoadDebugSetting()
		{
			bool debug = Utility.SettingsLoad<bool>("debug");
			Log.i(this, "Loading debugging state: " + debug);

			//We don't want to trigger the onClick event
			debuggingCheckBox.Click -= debuggingCheckBox_Click;
			debuggingCheckBox.IsChecked = debug;
			debuggingCheckBox.Click += debuggingCheckBox_Click;
		}

		void canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
		{
			if (FirstDraw)
			{
				FirstDraw = false;
				BeforeFirstDraw();
			}

			BackgroundImage.Draw(args.DrawingSession);
			BackgroundTaskStatus.Draw(args.DrawingSession);
		}

		//First Draw cycle of a Field calls this method
		void BeforeFirstDraw()
		{
			Log.i(this, "Operation before first draw started");

			//Creating Background instance
			BackgroundImage = new MainMenuBackground(this);

			//Initializing object for displaying current status of Background Tasks
			BackgroundTaskStatus = new BackgroundTaskStatus(10, 5);

			Log.i(this, "Operation before first draw finished");
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
			Log.i(this, "CreateResources starting parallel task for creating textures");
			args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());

			Log.i(this, "CreateResources finished");
		}

		//Loads all resources asynchronously
		async Task CreateResourcesAsync(CanvasAnimatedControl sender)
		{
			//Loads textures asynchronously
			await TextureSetLoader.Instance.CreateResourcesAsync
			(
				sender
				//Increases progress bar percentage during loading
				, (increasePercentage) => { loadingProgressBar.Value += increasePercentage; }
				//If not null, gets called back after loading gets finished
				, null
				//Only loads the required backgrounds for faster loading time
				, MainMenuBackground.backgrounds
			);

			//Music is also a resource
			Log.i(this, "Loading music");
			Music = await Utility.GetMusic(PageMusic);

			//Music is implicitly started, but to be sure, we explicitly start it
			//BUG: Music is not being looped
			LoadVolume();
			Music.Play();
			Music.IsLooping = true;

			Log.i(this, "Setting Field as loaded");
			FieldLoaded = true;

			Log.i(this, "CreateResourcesAsync finished");
		}

		public void AddActor(IAct actor)
		{
			throw new NotImplementedException();
		}
		public void RemoveActor(IAct actor)
		{
			throw new NotImplementedException();
		}

		public void ForEachActor<ActorType>(ActorAction<ActorType> action)
		{
			throw new NotImplementedException();
		}

		public void GameOver()
		{
			throw new NotImplementedException();
		}

		private void newGameButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "User clicked on New Game Button");

			//New game can only be started if the Main Menu is fully loaded
			if (!FieldLoaded)
			{
				return;
			}

			//The literally only way I could find to prevent music from loading and running even after it was stopped before old Page disposal
			if (Music.CurrentState == MediaElementState.Opening)
			{
				return;
			}

			Frame.Navigate(typeof(GameRound));
		}

		void settingsButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "User clicked on Settings Button");

			//Displayed page gets changed
			MainMenuMainScreenGrid.Visibility = Visibility.Collapsed;
			MainMenuSettingsGrid.Visibility = Visibility.Visible;
		}

		void exitButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "User clicked on Exit Game Button");

			//Version that crashed the application with exception: Window.Current.CoreWindow.Close();
			Utility.ExitGame(this);
		}

		void settingsReturnButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "User clicked on Return to Main Menu Button");

			//Displayed page gets changed back
			MainMenuMainScreenGrid.Visibility = Visibility.Visible;
			MainMenuSettingsGrid.Visibility = Visibility.Collapsed;

			//Settings get saved
			string resolutionWidthString = settingsResolutionWidth.Text.ToString();
			string resolutionHeightString = settingsResolutionHeight.Text.ToString();
			bool resolutionFullscreenBool = settingsFullscreen.IsChecked.Value;

			Utility.SettingsSave("resolutionWidth", resolutionWidthString);
			Utility.SettingsSave("resolutionHeight", resolutionHeightString);
			Utility.SettingsSave("resolutionFullscreen", resolutionFullscreenBool);
			Utility.SettingsSave("resultVolume", settingsVolume.Value);

			//Current resolution gets re-evaluated for the immediate feedback
			LoadScreenResolution();
		}

		void settingsResetToDefault_Click(object sender, RoutedEventArgs e)
		{
			//Saves invalid data to all fields
			Utility.SettingsSave<object>("resolutionFullscreen", null);
			Utility.SettingsSave<object>("resolutionWidth", null);
			Utility.SettingsSave<object>("resolutionHeight", null);
			Utility.SettingsSave<object>("resultVolume", null);

			//Reloads all data
			LoadScreenResolution();
			LoadSoundVolume();
		}

		void NewPlayerButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "User clicked on New player Button");
			Frame.Navigate(typeof(NewPlayer));
		}

		async void DeletePlayerButton_Click(object sender, RoutedEventArgs e)
		{
			var selectedItem = PlayersComboBox.SelectedItem as Services.Entity.Player;
			if (selectedItem == null)
			{
				Log.e(this, "Selected player is null");
				return;
			}

			await PopupDialog.ShowPopupDialog
				(
				"Are you sure you want to delete player " + selectedItem.Name + "?"
				, "Sure"
				, () =>
				{
					using (var db = new Persistence())
					{
						var players = db.Players;
						if (players == null)
						{
							Log.e(this, "Players in Database returned null");
							return;
						}

						players.Remove(selectedItem);
						db.SaveChanges();
						UpdatePlayers(players);
						Log.i(this, "Player removed");
					}
				}
				, "NO, it was a misclick!"
				, null
				);
		}

		void PlayersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var player = PlayersComboBox.SelectedItem as Services.Entity.Player;

			//This happens as a callback after player gets deleted, too.
			AfterSelectedPlayer(player);
		}

		async void debuggingCheckBox_Click(object sender, RoutedEventArgs e)
		{
			bool? debugging = debuggingCheckBox.IsChecked;
			Log.i(this, "Changed state of Debugging to " + debugging.ToString());
			Utility.SettingsSave("debug", debugging);

			//Offer the immediate change
			await PopupDialog.ShowPopupDialog
			(
				"Change will take place after next restart.\nDo you want to exit the game now?"
				, "Yes"
				, () =>
				{
					Utility.ExitGame(this);
				}
				, "No"
				, null
				, false
				, "Yes + schedule restart in 15 minutes"
				, async () =>
				{
					//Supposedly should be called before registering any background task, TODO: verify and integrate into schedule function
					await BackgroundExecutionManager.RequestAccessAsync();

					var restarter = BackgroundTask.ScheduleOneTime("GameRestarter", 15);
					Utility.ExitGame(this);
				}
			);
		}

		async void browseLogsButton_Click(object sender, RoutedEventArgs e)
		{
			string metroLogPath = Utility.LocalFolder.Path + @"\MetroLogs";
			Log.i(this, "Opening Logs folder as requested by user. Full path is: " + metroLogPath);

			//Launching the folder using the default folder viewer (Explorer)
			await Windows.System.Launcher.LaunchFolderAsync(await Windows.Storage.StorageFolder.GetFolderFromPathAsync(metroLogPath));
		}

		void removeTasksButton_Click(object sender, RoutedEventArgs e)
		{
			int taskCount = 0;
			foreach (var task in BackgroundTaskRegistration.AllTasks)
			{
				task.Value.Unregister(true);
				taskCount++;
			}
			Log.i(this, taskCount.ToString() + " tasks removed.");
		}
	}
}
