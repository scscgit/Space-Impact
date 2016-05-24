using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Space_Impact.Core.Game.ActorStrategy;

namespace Space_Impact.Core.Game.PartActor
{
	/// <summary>
	/// A.K.A "Part" Impl.
	/// </summary>
	public abstract class AbstractPartActor : AbstractActor, IActorCompositePart
	{
		//Ower of the part
		protected IActor Owner;

		//Lists that the Part is contained in
		LinkedList<LinkedList<IActorCompositePart>> containedInLists = new LinkedList<LinkedList<IActorCompositePart>>();

		/// <summary>
		/// Standard constructor.
		/// </summary>
		/// <param name="name">Name of the (Part) Actor</param>
		protected AbstractPartActor(IActor owner, string name) : base(owner.Name + "'s " + name)
		{
			Owner = owner;
			RegisterPartIn(owner);
		}

		//Registers the Part as an Actor in any list of Parts
		protected void RegisterPartIn(IActor containedInActor)
		{
			LinkedList<IActorCompositePart> containedInList = containedInActor.ActorComposition;
			containedInList.AddLast(this);
			this.containedInLists.AddLast(containedInList);

			//If the part is added to an Actor contained in Field, the part will be expected to be in the same Field too
			if (containedInActor.Field != null)
			{
				containedInActor.Field.AddActor(this);
			}
		}

		//Unregisters Part from all currently registered lists of Parts
		public void UnregisterPart()
		{
			foreach (LinkedList<IActorCompositePart> listOfParts in this.containedInLists)
			{
				listOfParts.Remove(this);
			}

			//Re-initializes the list, assuming the Part can be re-registered again later.
			this.containedInLists = new LinkedList<LinkedList<IActorCompositePart>>();
		}

		//Parts do never collide
		public override bool CollidesWith(IActor actor)
		{
			return false;
		}
	}
}
