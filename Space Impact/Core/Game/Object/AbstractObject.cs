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

		//All objects disappear when they leave the Field
		protected override bool OutOfFieldBounds()
		{
			return X < -Field.Size.Width || X > Field.Size.Width || Y < -Field.Size.Height || Y > Field.Size.Height;
		}

		//Objects implicitly do not collide unless explicitly overridden
		public override bool CollidesWith(IActor actor)
		{
			return false;
		}
	}
}
