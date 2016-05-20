using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Support
{
	/**
	 * Defines methods for an easy queue of Actions.
	 * This system can be included in any class by extending from it.
	 * Each call of runActions() method takes Input, which gets passed to other actions and returns Output.
	 * <p/>
	 * Created by Steve on 24.12.2015.
	 */
	public class ActionQueue<Input, Output>
	{
		AbstractAction<Input, Output> first;
		AbstractAction<Input, Output> last;

		public ActionQueue()
		{
			resetActions();
		}

		//Deletes all actions and lets the garbage collector do the job it was meant to do
		public ActionQueue<Input, Output> resetActions()
		{
			this.first = null;
			//this.last is implicitly deleted when this.first becomes null
			return this;
		}

		//Adds a new action to the "linked" list
		public ActionQueue<Input, Output> addAction(AbstractAction<Input, Output> action)
		{
			if (this.first == null)
			{
				//If no action was added yet, the action will be a first action
				this.first = action;
				this.last = action;
			}
			else
			{
				this.last.setNextAction(action);
				this.last = action;
			}
			return this;
		}

		//Runs all linked actions on target type object
		public Output runActions(Input target)
		{
			if (this.first != null)
			{
				return this.first.doAction(target);
			}
			return default(Output);
		}

		//Implicitly runs actions without any object of interest in the input.
		//It is advised to override this by some default behavior.
		public Output runActions()
		{
			return runActions(default(Input));
		}
	}
}
