using Space_Impact.Core.Game.Object;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Spawner
{
	public class EnemySpawner: AbstractSpawner
	{
		public EnemySpawner(IField field, float x, float y): base(field, x, y)
		{
			RemainingEnemies = 5;
		}

		protected override void Spawn()
		{
			Log.i(this, "Spawn() called");
			var enemy = new Doomday(Field.Player);
			enemy.X = Position.X;
			enemy.Y = Position.Y;
			Field.AddActor(enemy);
		}

		public override void Act()
		{
			base.Act();
		}
	}
}
