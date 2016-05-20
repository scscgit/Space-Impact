using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Object
{
	public abstract class AbstractObject : AbstractActor, IObject
	{
		protected AbstractObject(string name) : base(name)
		{
		}

		//Removes all movement limitations, instead verifies in the Act whether the object needs to be destroyed
		protected override bool CanMoveX(float x)
		{
			return true;
		}
		protected override bool CanMoveY(float y)
		{
			return true;
		}

		bool OutOfFieldBounds()
		{
			return X < -Width || X > Field.Size.Width || Y < -Height || Y > Field.Size.Height;
		}

		public override void Act()
		{
			base.Act();

			if (OutOfFieldBounds())
			{
				DeleteActor();
				Log.i(this, Name + " outside of the map, removed");
			}
		}
	}
}
