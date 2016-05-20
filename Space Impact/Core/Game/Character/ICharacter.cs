using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Character
{
	public interface ICharacter: IActor
	{
		//User-friendly properties of a character
		int MaxHealth
		{
			get;
		}
		int Health
		{
			get; set;
		}
	}
}
