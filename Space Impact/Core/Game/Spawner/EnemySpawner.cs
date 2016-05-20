using Space_Impact.Core.Game.Object;
using Space_Impact.Core.Game.Spawner.Strategy;
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
		//Constant settings
		private const int REMAINING_ENEMIES = 50;

		public EnemySpawner(IField field, float x, float y): base(field, x, y, REMAINING_ENEMIES)
		{
			//Strategies used within the Spawner
			Strategies.Add(new EveryNActs(this, 100));
			Strategies.Add(new EveryNPercent(this, 1));
		}

		protected override void SpawnCallback()
		{
			Log.i(this, "SpawnCallback() called");
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
