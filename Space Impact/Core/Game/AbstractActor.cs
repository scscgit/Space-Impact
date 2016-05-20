using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Space_Impact.Core.Game;

namespace Space_Impact.Core
{
	public abstract class AbstractActor : AnimatedObject, IActor
	{
		public const int DEFAULT_SPEED = 1;

		IField field = null;
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

		public SpaceDirection Direction
		{
			get; set;
		}

		public int Speed
		{
			get; set;
		}

		protected AbstractActor(string name)
		{
			Direction = SpaceDirection.get(SpaceDirection.HorizontalDirection.NONE, SpaceDirection.VerticalDirection.NONE);
			Speed = DEFAULT_SPEED;
			Name = name;
		}
		
		/// <summary>
		/// Update operation called before each Draw
		/// </summary>
		public virtual void Act()
		{
			if (Direction.Horizontal == SpaceDirection.HorizontalDirection.LEFT)
			{
				X = X - Speed;
			}
			else if(Direction.Horizontal == SpaceDirection.HorizontalDirection.RIGHT)
			{
				X = X + Speed;
			}

			if (Direction.Vertical == SpaceDirection.VerticalDirection.UP)
			{
				Y = Y - Speed;
			}
			else if (Direction.Vertical == SpaceDirection.VerticalDirection.DOWN)
			{
				Y = Y + Speed;
			}
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
