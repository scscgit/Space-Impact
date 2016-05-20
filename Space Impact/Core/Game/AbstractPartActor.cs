using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Player
{
	/// <summary>
	/// A.K.A "Part" Impl.
	/// </summary>
	public abstract class AbstractPartActor : AbstractActor, IActorCompositePart
	{
		//Lists that the Part is contained in
		LinkedList<LinkedList<IActorCompositePart>> containedInLists = new LinkedList<LinkedList<IActorCompositePart>>();

		/// <summary>
		/// Standard constructor.
		/// </summary>
		/// <param name="name">Name of the (Part) Actor</param>
		protected AbstractPartActor(string name) : base(name)
		{
		}

		//Registers the Part as an Actor in any list of Parts
		public void RegisterPartIn(IActor containedInActor)
		{
			LinkedList<IActorCompositePart> containedInList = containedInActor.ActorComposition;
			containedInList.AddLast(this);
			this.containedInLists.AddLast(containedInList);
		}

		//Unregisters Part from all currently registered lists of Parts
		public void UnregisterPart()
		{
			foreach(LinkedList<IActorCompositePart> listOfParts in this.containedInLists)
			{
				listOfParts.Remove(this);
			}

			//Re-initializes the list, assuming the Part can be re-registered again later.
			this.containedInLists = new LinkedList<LinkedList<IActorCompositePart>>();
		}
	}
}
