﻿using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Player
{
	public interface IPlayer : IActor
	{
		bool Shooting { get; set; }
		Position BulletFocusPosition { get; }
	}
}
