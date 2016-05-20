using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core
{
	public abstract class AbstractActor : AnimatedObject, IActor
	{
		private IField field = null;
		public IField Field
		{
			get
			{
				return field;
			}
			private set
			{
				field = value;
			}
		}

		protected AbstractActor()
		{

		}

		
		public virtual void Act()
		{

		}

		public void RemoveFromField()
		{
			Field.RemoveActor(this);
		}

		public void AddedToField(IField field)
		{
			Field = field;
		}

		public virtual void AddedToFieldHook()
		{

		}
	}
}
