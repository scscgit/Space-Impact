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
	public interface IField
	{
		CanvasAnimatedControl FieldControl { get; }
		IPlayer Player { get; }
		Size Size
		{ get; }

		void AddActor(IActor actor);
		void RemoveActor(IActor actor);
	}
}
