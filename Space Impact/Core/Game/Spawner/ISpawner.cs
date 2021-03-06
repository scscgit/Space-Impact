﻿using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Spawner
{
	public delegate void SpawnCallbackDelegate(ISpawner spawner);

	/// <summary>
	/// Spawner is also like an Actor, but does not implement Actor interface, only Act.
	/// He should Spawn Actors to the Field.
	/// </summary>
	public interface ISpawner : IAct, IPlacedInSpace, IPlacedInField
	{
		/// <summary>
		/// Returns a current representative target position for the Spawner.
		/// Main access point to the Spawner's position of Spawned Actors.
		/// X and Y should not be used directly, as they represent him only as an Actor, not as a Spawner with custom rules.
		/// </summary>
		Position Position { get; }

		IField Field { get; }
		int RemainingEnemies { get; }
		bool UnlimitedEnemies { get; }
		bool Active { get; }

		void PlaceActor(IActor actor);

		void Spawn(SpawnCallbackDelegate spawnCallback);
	}
}
