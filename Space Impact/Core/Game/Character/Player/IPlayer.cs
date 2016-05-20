using Space_Impact.Core.Game.Character;
using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Player
{
	public interface IPlayer : ICharacter
	{
		

		//todo weapon


		//Internal game logic
		bool Shooting { get; set; }
		Position BulletFocusPosition { get; }
	}
}
