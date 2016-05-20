using Space_Impact.Core.Game.Enemy;
using Space_Impact.Core.Game.Object.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Object.Bomb
{
	public interface IBomb : IEnemy
	{
		int Damage
		{
			get;
		}
		//void Explode();
		//bool EnemyWithinRange(float x, float y);
	}
}
