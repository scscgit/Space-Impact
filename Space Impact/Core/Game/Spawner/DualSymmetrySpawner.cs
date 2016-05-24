using Space_Impact.Core.Game.Character.Enemy;
using Space_Impact.Core.Game.Character.Enemy.Bomb;
using Space_Impact.Core.Game.Object;
using Space_Impact.Core.Game.Spawner.Wrapper;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Spawner
{
	/// <summary>
	/// Adds two symmetrical Spawners to the Field, but hides (and controls) their Acts using a HideAct wrapper.
	/// </summary>
	public class DualSymmetrySpawner : AbstractSpawner
	{
		//Symmetrical Spawners
		HideAct LeftSpawner = null;
		HideAct RightSpawner = null;

		//Settings
		int RemainingEnemiesLeft;
		int RemainingEnemiesRight;

		SpawnCallbackDelegate LeftSpawnCallback;
		SpawnCallbackDelegate RightSpawnCallback;

		public DualSymmetrySpawner(float y, int remainingEnemiesLeft, int remainingEnemiesRight, SpawnCallbackDelegate leftSpawnCallback, SpawnCallbackDelegate rightSpawnCallback)
			: base(0, y, remainingEnemiesLeft + remainingEnemiesRight)
		{
			RemainingEnemiesLeft = remainingEnemiesLeft;
			RemainingEnemiesRight = remainingEnemiesRight;
			SetSpawnCallback(leftSpawnCallback, rightSpawnCallback);
		}

		//Implicitly uses equal callback on both sides
		public DualSymmetrySpawner(float y, int remainingEnemiesLeft, int remainingEnemiesRight, SpawnCallbackDelegate equalSpawnCallback)
			: this(y, remainingEnemiesLeft, remainingEnemiesRight, equalSpawnCallback, equalSpawnCallback)
		{
		}

		/// <summary>
		/// Sets new Callbacks, overwriting old ones.
		/// This is useful for delayed construction of the Spawner when its fields are required within the callback definition.
		/// </summary>
		/// <param name="leftSpawnCallback">callback used in the left Spawner</param>
		/// <param name="rightSpawnCallback">callback used in the right Spawner</param>
		public void SetSpawnCallback(SpawnCallbackDelegate leftSpawnCallback, SpawnCallbackDelegate rightSpawnCallback)
		{
			LeftSpawnCallback = leftSpawnCallback;
			RightSpawnCallback = rightSpawnCallback;
		}

		public void SetSpawnCallback(SpawnCallbackDelegate equalSpawnCallback)
		{
			SetSpawnCallback(equalSpawnCallback, equalSpawnCallback);
		}

		public override void AddedToFieldHook()
		{
			base.AddedToFieldHook();

			LeftSpawner = new HideAct(new EnemySpawner(0, Y, RemainingEnemiesLeft, LeftSpawnCallback));
			RightSpawner = new HideAct(new EnemySpawner((float)Field.Size.Width, Y, RemainingEnemiesRight, RightSpawnCallback));

			Field.AddActor(LeftSpawner);
			Field.AddActor(RightSpawner);
		}

		public override void DeleteActorHook()
		{
			base.DeleteActorHook();

			LeftSpawner.DeleteActor();
			RightSpawner.DeleteActor();

			LeftSpawner = null;
			RightSpawner = null;
		}

		public override void Act()
		{
			base.Act();
			if (LeftSpawner != null)
			{
				LeftSpawner.HiddenAct();
			}
			if (RightSpawner != null)
			{
				RightSpawner.HiddenAct();
			}
		}
	}
}
