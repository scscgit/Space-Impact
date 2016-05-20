using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Support
{
	/**
	 * Ready, set, action!
	 * Chain of responsibility superclass.
	 * Supports input type (that each action receives) and output type (that only gets returned once after chain
	 * execution).
	 * <p/>
	 * Created by Steve on 23.11.2015.
	 */
	public abstract class AbstractAction<Input, Output>
	{
		/*protected*/
		AbstractAction<Input, Output> nextAction;

		public AbstractAction()
		{
			this.nextAction = null;
		}

		//Nastavi novu nasledujucu akciu pre (aktualnu) danu akciu
		public void setNextAction(AbstractAction<Input, Output> action)
		{
			this.nextAction = action;
		}

		//Ak existuje nasledujuca akcia, tak sa spusti
		public Output doNextAction(Input actor)
		{
			if (this.nextAction != null)
			{
				return nextAction.doAction(actor);
			}
			return default(Output);
		}

		//Kazda akcia by mala spustat nasledujucu akciu, no zalezi na implementacii.
		//Vratenim hodnoty typu Output sa skonci retazec vykonavania.
		public Output doAction(Input actor)
		{
			return doNextAction(actor);
		}
	}
}
