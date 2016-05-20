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

		public string Name
		{
			get; set;
		}

		protected AbstractActor(string name)
		{
			Name = name;
		}
		
		/// <summary>
		/// Update operation called before each Draw
		/// </summary>
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

		/// <summary>
		/// Hook callback for initialization operations after being connected to a Field
		/// </summary>
		public virtual void AddedToFieldHook()
		{

		}
	}
}
