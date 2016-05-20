﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Object.Bomb
{
	public interface IAffectedByBombExplosion
	{
		void OnBombExplosion(IBomb bomb);
	}
}
