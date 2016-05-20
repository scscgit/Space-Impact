using Microsoft.Graphics.Canvas.UI.Xaml;
using Space_Impact.Core.Game.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Space_Impact.Core
{
	public delegate bool ActorAction(IActor actor);

	public interface IField
	{
		CanvasAnimatedControl FieldControl { get; }
		IPlayer Player { get; }
		Size Size { get; }
		string MessageBroadcastText { get; set; }

		//Actor management
		void AddActor(IActor actor);
		void RemoveActor(IActor actor);
		void ForEachActor(ActorAction action);

		//Gameplay flow management
		void GameOver();
	}
}
