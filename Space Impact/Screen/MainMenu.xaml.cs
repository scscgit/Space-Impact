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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Space_Impact.Screen
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainMenu : Page, IField
	{
		const string PageMusic = "userspace"; //"vault";

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

		public MainMenu()
		{
			Log.i(this, "Constructor initializing");

			this.InitializeComponent();

			//Initializing loading screen
			FieldLoaded = false;

			//Preparing Main Screen of MainMenu grid
			MainMenuMainScreenGrid.Visibility = Visibility.Visible;
			MainMenuSettingsGrid.Visibility = Visibility.Collapsed;

			LoadScreenResolution();

			double? resultVolume = Utility.SettingsLoad<double?>("resultVolume");
			if(resultVolume == null)
			{
				resultVolume = 80;
			}
			settingsVolume.Value = resultVolume.Value;
			settingsVolume.ValueChanged += (a, b) => LoadVolume();

			Log.i(this, "Constructor initialized");
		}

		/// <summary>
		/// Initializes screen resolution view size from settings.
		/// Loads default values when the settings are not found.
		/// Updates corresponding values in the Settings submenu.
		/// </summary>
		private void LoadScreenResolution()
		{
			//Resolution of the game screen
			int resolutionWidth;
			int resolutionHeight;
			string resolutionWidthString = Utility.SettingsLoad<string>("resolutionWidth");
			string resolutionHeightString = Utility.SettingsLoad<string>("resolutionHeight");

			bool resolutionFullscreen = Utility.SettingsLoad<bool>("resolutionFullscreen");

			//Parsing values from settings, setting both to default on error
			bool resultWidth = int.TryParse(resolutionWidthString, out resolutionWidth);
			bool resultHeight = int.TryParse(resolutionHeightString, out resolutionHeight);
			if (!resultWidth || !resultHeight)
			{
				resolutionWidth = 1600;
				resolutionHeight = 900;
			}

			Log.i(this, "Loading width resolution from settings: " + (resultWidth ? "success" : "failure") + ", used " + resolutionWidth.ToString());
			Log.i(this, "Loading height resolution from settings: " + (resultHeight ? "success" : "failure") + ", used " + resolutionHeight.ToString());
			Log.i(this, "Loading fullscreen choice from settings: " + (resolutionFullscreen ? "true" : "false"));

			//Applying the values to the current window
			Size windowSize = new Size(resolutionWidth, resolutionHeight);
			var view = ApplicationView.GetForCurrentView();

			//Force-resize on top of the preferred setting in case the function is called again (after settings get changed)
			ApplicationView.PreferredLaunchViewSize = windowSize;
			view.TryResizeView(windowSize);
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

			//Updating values in the Settings submenu
			settingsResolutionWidth.Text = resolutionWidth.ToString();
			settingsResolutionHeight.Text = resolutionHeight.ToString();
			settingsFullscreen.IsChecked = resolutionFullscreen;
		}

		private void LoadVolume()
		{
			if (Music != null)
			{
				Music.Volume = settingsVolume.Value;
			}
		}

		void canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
		{
			if (FirstDraw)
			{
				FirstDraw = false;
				OnFirstDraw();
			}

			BackgroundImage.Draw(args.DrawingSession);
		}

		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			Log.i(this, "Page is being unloaded, removing events and other associations");
			Log.i(this, "Music state before stopping was " + Music.CurrentState.ToString());

			Music.Stop();

			//Cleaning up the Page to help the garbage collector
			canvas.RemoveFromVisualTree();
			canvas = null;
			Log.i(this, "Page was fully unloaded");
		}

		//First Draw cycle of a Field calls this method
		void OnFirstDraw()
		{
			Log.i(this, "First draw started");

			BackgroundImage = new MainMenuBackground(this);

			Log.i(this, "First draw finished");
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
				//Gets called back after loading gets finished
				, AfterCreateResourcesAsyncFinished
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

		void AfterCreateResourcesAsyncFinished()
		{


			Log.i(this, "AfterCreateResourcesAsyncFinished finished");
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

		private void settingsButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "User clicked on Settings Button");

			//Displayed page gets changed
			MainMenuMainScreenGrid.Visibility = Visibility.Collapsed;
			MainMenuSettingsGrid.Visibility = Visibility.Visible;
		}

		private void exitButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "User clicked on Exit Game Button");

			//Version that crashed the application with exception: Window.Current.CoreWindow.Close();
			Application.Current.Exit();
		}

		private void settingsReturnButton_Click(object sender, RoutedEventArgs e)
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
	}
}
