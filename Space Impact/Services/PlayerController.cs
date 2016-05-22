using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Services
{
	public static class PlayerController
	{
		/// <summary>
		/// Currently selected Player.
		/// </summary>
		public static Entity.Player Player
		{
			get; set;
		} = null;
	}
}
