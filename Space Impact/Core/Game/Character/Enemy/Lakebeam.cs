using Space_Impact.Core.Game.Enemy;
using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Character.Enemy
{
	public class Lakebeam : AbstractEnemy
	{
		public Lakebeam(string name) : base(name)
		{
			Animation = TextureSetLoader.SHIP2_BASE;
			Direction = SpaceDirection.None;
			Direction += SpaceDirection.VerticalDirection.DOWN;
		}

		public override void OnDeath()
		{
			//todo
			throw new NotImplementedException();
		}
	}
}
