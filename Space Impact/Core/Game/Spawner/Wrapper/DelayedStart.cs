using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Spawner.Wrapper
{
	/// <summary>
	/// Delays the operation of the Spawner.
	/// </summary>
	public class DelayedStart : AbstractSpawnerWrapper
	{
		int ActsDelay;
		int CurrentAct;
		float PercentDelay;
		float StartPercent;

		//Field, which the Spawner is being added to and thus, has its percentage tracked
		IField DelayedField;

		private bool deactivated;
		protected bool Deactivated
		{
			get
			{
				return deactivated;
			}
			set
			{
				//On activation, runs activated events
				if (value == false && ActivatedEvent != null)
				{
					ActivatedEvent();
					ActivatedEvent = null;
				}
				this.deactivated = value;
			}
		}

		protected delegate void ActivatedDelegate();
		protected event ActivatedDelegate ActivatedEvent;

		public DelayedStart(int actsDelay, float percentDelay, ISpawner spawner) : base(spawner)
		{
			ActsDelay = actsDelay;
			CurrentAct = 0;
			PercentDelay = percentDelay;

			if (Spawner.Field != null)
			{
				DelayedField = Spawner.Field;
				StartPercent = DelayedField.Percent;
			}
			else
			{
				//If the Spawner is not in world, we will initialize it to zero instead of crashing the game
				StartPercent = 0;
			}

			//Deactivates Spawner until he gets explicitly activated
			Deactivated = true;
		}

		public override void Act()
		{
			if (Deactivated)
			{
				//Count the required acts
				if (CurrentAct < ActsDelay)
				{
					CurrentAct++;
				}
				//After the required number of acts has been done, wait until the percentage requirement gets fulfulled
				else if (DelayedField.Percent - StartPercent >= PercentDelay)
				{
					//If the act is reached and the required delta percent gets increased too, Spawner gets activated
					Log.i(this, "DelayedStart has activated a Spawner");
					Deactivated = false;
				}
			}
			//Acts if not deactivated
			else
			{
				base.Act();
			}
		}

		public override void AddedToField(IField field)
		{
			DelayedField = field;

			//Schedules adding the actor to the Field
			ActivatedEvent = () => base.AddedToField(DelayedField);
		}

		public override void DeleteActor()
		{
			//Un-schedules possible actor that is about to be added to the Field
			ActivatedEvent = null;

			//Deletes the Actor (including preventive deletion)
			base.DeleteActor();
		}
	}
}
