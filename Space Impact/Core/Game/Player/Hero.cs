using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Player
{
	public class Hero : AbstractPlayer
	{
		MovementFlame f;

		public Hero()
		{
			string[] textureSet =
			{
				"ship1_base",
				"ship1_fire"
			};
			setAnimation(textureSet);

			f = new MovementFlame();
			

		}

		public override void Act()
		{
			base.Act();
		}

		public override void AddedToFieldHook()
		{
			Field.AddActor(f);
		}

		public override void DrawHook()
		{
			f.Draw();
		}
	}
}
