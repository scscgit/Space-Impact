using Space_Impact.Services;
using Space_Impact.Support;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Space_Impact.Screen
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class NewPlayer : Page
	{
		//Dimensions
		private const int MIN_PLAYER_NAME_LENGTH = 4;
		private const int MAX_PLAYER_NAME_LENGTH = 22;

		public NewPlayer()
		{
			this.InitializeComponent();
		}

		private void createPlayerButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "Create Player Button clicked");

			//Verifying the static validity of a name
			string playerName = playerNameTextBox.Text;
			if (playerName.Length < MIN_PLAYER_NAME_LENGTH)
			{
				errorTextBlock.Text = "Name is too short. Please add " + (MIN_PLAYER_NAME_LENGTH - playerName.Length).ToString() + " characters.";
			}
			else if (playerName.Length > MAX_PLAYER_NAME_LENGTH)
			{
				errorTextBlock.Text = "Name is too long. Please remove " + (playerName.Length - MAX_PLAYER_NAME_LENGTH).ToString() + " characters.";
			}
			else
			{
				Services.Entity.Player player = new Services.Entity.Player() { Name = playerName };

				using (var db = new Persistence())
				{
					var players = db.Players;
					if (players == null)
					{
						Log.e(this, "Players in Database returned null");
						return;
					}

					//Verifying collisions
					if (players.ToList().Exists(p => p.Name.Equals(playerName)))
					{
						errorTextBlock.Text = "Sorry, name " + playerName + " is already used.";
						return;
					}

					//Persisting the Player
					players.Add(player);
					db.SaveChanges();

					//After saving changes, the ID in our entity was automatically generated
					//The new Player will be the selected Player
					Utility.SettingsSave<int?>("selectedPlayer", player.Id);

					Log.i(this, "Player " + player.Name + " added");
				}

				//If everything goes well, navigate back to Main Menu
				Frame.Navigate(typeof(MainMenu));
			}
		}

		private void returnButton_Click(object sender, RoutedEventArgs e)
		{
			Log.i(this, "Return without saving Button clicked");

			Frame.Navigate(typeof(MainMenu));
		}
	}
}
