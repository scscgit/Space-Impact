using Space_Impact.Core.Game.Character.Enemy;
using Space_Impact.Core.Game.Character.Enemy.Bomb;
using Space_Impact.Core.Game.Enemy;
using Space_Impact.Core.Game.Object;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Spawner
{
	public class DualSymmetrySpawner : AbstractSpawner
	{
		//Constant settings
		const int REMAINING_ENEMIES_LEFT = 10;
		const int REMAINING_ENEMIES_RIGHT = 15;

		//Symmetrical Spawners
		ISpawner LeftSpawner = null;
		ISpawner RightSpawner = null;

		public DualSymmetrySpawner(float y) : base(0, y, REMAINING_ENEMIES_LEFT + REMAINING_ENEMIES_RIGHT)
		{
		}

		public override void AddedToFieldHook()
		{
			base.AddedToFieldHook();

			LeftSpawner = new EnemySpawner(0, Y, REMAINING_ENEMIES_LEFT, LeftSpawnCallback);
			RightSpawner = new EnemySpawner((float)Field.Size.Width, Y, REMAINING_ENEMIES_RIGHT, RightSpawnCallback);

			Field.AddActor(LeftSpawner);
			Field.AddActor(RightSpawner);
		}

		public override void DeleteActorHook()
		{
			base.DeleteActorHook();

			LeftSpawner = null;
			RightSpawner = null;

			Field.RemoveActor(LeftSpawner);
			Field.RemoveActor(RightSpawner);
		}

		protected void LeftSpawnCallback()
		{
			Log.i(this, "LeftSpawnCallback() called");
			IEnemy enemy = Utility.RandomBetween(0, 1) == 1 ? new Doomday(Field.Player) : null;
			if (enemy == null) enemy = new Lakebeam();
			enemy.X = LeftSpawner.Position.X;
			enemy.Y = LeftSpawner.Position.Y;
			Field.AddActor(enemy);
		}

		protected void RightSpawnCallback()
		{
			Log.i(this, "RightSpawnCallback() called");
			IEnemy enemy = Utility.RandomBetween(0, 1) == 1 ? new Doomday(Field.Player) : null;
			if (enemy == null) enemy = new Lakebeam();
			enemy.X = RightSpawner.Position.X;
			enemy.Y = RightSpawner.Position.Y;
			Field.AddActor(enemy);
		}
	}
}
