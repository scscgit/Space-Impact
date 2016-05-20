using Microsoft.Graphics.Canvas.UI.Xaml;
using Space_Impact.Core.Game;
using Space_Impact.Core.Game.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Space_Impact.Core
{
	public delegate bool ActorAction<ActorType>(ActorType actor);

	public interface IField
	{
		bool FieldLoaded { get; }
		bool GameRunning { get; }
		CanvasAnimatedControl FieldControl { get; }
		IPlayer Player { get; }
		Size Size { get; }
		string MessageBroadcastText { get; set; }
		float Percent { get; }

		//Actor management
		void AddActor(IAct actor);
		void RemoveActor(IAct actor);
		void ForEachActor<ActorType>(ActorAction<ActorType> action);

		//Gameplay flow management
		void GameOver();
	}
}
