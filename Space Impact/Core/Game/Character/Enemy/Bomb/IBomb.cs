using Space_Impact.Core.Game.Character.Enemy;
using Space_Impact.Core.Game.Object.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Character.Enemy.Bomb
{
	public interface IBomb : IEnemy
	{
		/// <summary>
		/// State representing the already initiated explosion.
		/// </summary>
		bool Exploding
		{
			get;
		}

		/// <summary>
		/// Damage that the bomb deals on the explosion.
		/// </summary>
		int Damage
		{
			get;
		}
		//void Explode();
		//bool EnemyWithinRange(float x, float y);
	}
}
