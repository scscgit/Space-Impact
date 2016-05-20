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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Space_Impact.Screen
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainMenu : Page, IField
	{
		/// <summary>
		/// Background class of the MainMenu.
		/// </summary>
		internal class MainMenuBackground : AbstractBackground
		{
			public MainMenuBackground(IField field) : base(field)
			{
				string[][] backgrounds = new string[][]
				{
					TextureSetLoader.BG_CRATER_PLANET, TextureSetLoader.BG_MESSIER,
					TextureSetLoader.BG_ROCKET_AND_PLANET, TextureSetLoader.BG_STAR_CLUSTERS
				};
				
				Animation = backgrounds[Utility.RandomBetween(0, backgrounds.Length - 1)];
				Speed = 5;
				AddStrategy(new RandomMovement(this, field));
			}
		}

		IBackground BackgroundImage;
		MediaElement Music;

		//Represents if the screen is loaded (fully initialized) with all resources and can be safely navigated out from.
		public bool GameRunning
		{
			get; private set;
		}

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
			GameRunning = false;
			Log.i(this, "Constructor initialized");
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

			GameRunning = true;
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
			await TextureSetLoader.Instance.CreateResourcesAsync(sender);

			//Music is also a resource
			Music = await Utility.GetMusic("vault");
			Music.IsLooping = true;
			//Music is implicitly started, but to be sure, we explicitly start it
			Music.Play();
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
			/*if (!GameRunning)
			{
				return;
			}*/

			Frame.Navigate(typeof(GameRound));
		}

		private void settingsButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "User clicked on Settings Button");
		}

		private void exitButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "User clicked on Exit Game Button");

			//Version that crashed the application with exception: Window.Current.CoreWindow.Close();
			Application.Current.Exit();
		}
	}
}
