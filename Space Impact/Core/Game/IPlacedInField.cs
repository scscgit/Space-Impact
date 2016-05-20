using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core
{
	public interface IPlacedInField
	{
		/// <summary>
		/// Adds the current actor to a Field.
		/// </summary>
		/// <param name="field">Field, to which the actor is being added</param>
		void AddedToField(IField field);
		/// <summary>
		/// Hook callback for initialization operations after being connected to a Field.
		/// </summary>
		void AddedToFieldHook();

		/// <summary>
		/// Removes the current actor from the Field.
		/// </summary>
		void DeleteActor();
		/// <summary>
		/// Hook callback for destruction operations after being disconnected from a Field.
		/// </summary>
		void DeleteActorHook();
	}
}
