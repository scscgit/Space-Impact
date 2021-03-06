﻿using Space_Impact.Core.Game.Character;
using Space_Impact.Core.Game.Object.Collectable;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Space_Impact.Core.Game.Player;
using Space_Impact.Services;

namespace Space_Impact.Core.Game.Character.Enemy
{
	public abstract class AbstractEnemy : AbstractCharacter, IEnemy
	{
		private int score;
		public int Score
		{
			get
			{
				return score;
			}
			protected set
			{
				if (value >= 0)
				{
					score = value;
				}
				else
				{
					score = 0;
				}
			}
		}

		protected AbstractEnemy(string name, int score) : base(name)
		{
			Score = score;
		}

		//Enemies can implicitly fall out of the visible zone
		protected override bool CanMoveX(float x)
		{
			return true;
		}
		protected override bool CanMoveY(float y)
		{
			return true;
		}

		//Enemies implicitly disappear after they fall below the visible zone
		protected override bool OutOfFieldBounds()
		{
			return Y > Field.Size.Height;
		}

		/// <summary>
		/// Subclass may implement loot to be dropped on death.
		/// </summary>
		/// <returns>Collectable object that will be added on the Field</returns>
		protected virtual ICollectable DropLoot()
		{
			return null;
		}

		/// <summary>
		/// Event of the character dying.
		/// </summary>
		public override void OnDeath()
		{
			//If the character has any loot to be dropped, it drops it on the same coordinates
			ICollectable drop = DropLoot();
			if (drop != null)
			{
				Log.i(this, Name + " dropped loot " + drop.Name);
				AddActorToSameCoordinates(drop);
			}
			else
			{
				Log.i(this, Name + " died without any loot");
			}

			//If the game is not lost, the Player will get the credit for the kill
			if (Field.GameRunning)
			{
				//The thread will run in parallel, and we won't query the database in a game loop, so there won't be lags
				PlayerController.AddScore(Score);
			}

			RemoveFromField();
		}
	}
}
