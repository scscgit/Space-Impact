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
			//First greeting enemy (plan is to display a greeting message using black_old_face.png within the UI)
			Spawners.Add
			(
				new EnemySpawner
				(
					x: (float)Field.Size.Width / 2
					, y: 20
					, remainingEnemies: 1
					, spawnCallback: spawner => spawner.PlaceActor(new Lakebeam())
				)
			);

			//First Bomb to give Player some small target to fire on
			AddDualSpawner
			(
				percentDelay: 2
				, enemies: 1
				, spawnCallback: spawner =>
				{
					var enemy = new Doomday(Field.Player);
					spawner.PlaceActor(enemy);
				}
			);

			//Wave of Lakebeams for the opportunity of quickly upgrading the basic weapon
			AddDualSpawner
			(
				percentDelay: 6
				, enemies: 4
				, spawnCallback: spawner =>
				{
					var enemy = new Lakebeam();
					spawner.PlaceActor(enemy);
				}
			);

			//Wave of bombs
			AddDualSpawner
			(
				percentDelay: 16
				, enemies: 5
				, spawnCallback: spawner =>
				{
					var enemy = new Doomday(Field.Player);
					spawner.PlaceActor(enemy);
				}
			);

			//Two Venomflares to make it more interesting
			Spawners.Add
			(
				new DelayedStart
				(
					actsDelay: 0
					, percentDelay: 26
					, spawner: new EnemySpawner
					(
						//Spawns in the 1/3 of the Field
						x: (float)Field.Size.Width / 3
						, y: 0
						, remainingEnemies: 1
						, spawnCallback: spawner => spawner.PlaceActor(new Venomflare())
					)
				)
			);
			Spawners.Add
			(
				new DelayedStart
				(
					actsDelay: 0
					, percentDelay: 26
					, spawner: new EnemySpawner
					(
						//Spawns in the 2/3 of the Field
						x: 2 * (float)Field.Size.Width / 3
						, y: 0
						, remainingEnemies: 1
						, spawnCallback: spawner => spawner.PlaceActor(new Venomflare())
					)
				)
			);

			//Waveghosts that will rotate around
			AddDualSpawner
			(
				percentDelay: 36
				, y: (float)Field.Size.Height / 2
				, enemies: 2
				, spawnCallback: spawner => spawner.PlaceActor(new Waveghost())
			);

			//Larger wave of Lakebeams
			AddDualSpawner
			(
				percentDelay: 50
				, enemies: 8
				, spawnCallback: spawner =>
				{
					var enemy = new Lakebeam();
					spawner.PlaceActor(enemy);
				}
			);

			//Last small wave of bombs
			AddDualSpawner
			(
				percentDelay: 60
				, enemies: 4
				, spawnCallback: spawner =>
				{
					var enemy = new Doomday(Field.Player);
					spawner.PlaceActor(enemy);
				}
			);

			//Final battle before boss appears
			AddDualSpawner
			(
				percentDelay: 75
				, y: (float)Field.Size.Height / 2
				, enemies: 4
				, spawnCallback: spawner => spawner.PlaceActor(new Waveghost())
			);
		}
	}
}
