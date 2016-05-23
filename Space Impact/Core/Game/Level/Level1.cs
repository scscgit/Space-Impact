using Space_Impact.Core.Game.Character.Enemy;
using Space_Impact.Core.Game.Character.Enemy.Bomb;
using Space_Impact.Core.Game.Spawner;
using Space_Impact.Core.Game.Spawner.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Level
{
	public class Level1 : AbstractLevel
	{
		override protected void Construct()
		{
			FirstWave();
		}

		private void FirstWave()
		{
			AddDualSpawner
			(
				percentDelay: 3
				, enemies: 1
				, spawnCallback: spawner =>
				{
					var enemy = new Doomday(Field.Player);
					spawner.PlaceActor(enemy);
				}
			);

			AddDualSpawner
			(
				percentDelay: 7
				, enemies: 3
				, spawnCallback: spawner =>
				{
					var enemy = new Lakebeam();
					spawner.PlaceActor(enemy);
				}
			);

			AddDualSpawner
			(
				percentDelay: 12
				, enemies: 5
				, spawnCallback: spawner =>
				{
					var enemy = new Doomday(Field.Player);
					spawner.PlaceActor(enemy);
				}
			);
		}
	}
}
