using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Space_Impact.Core.Game;
using Space_Impact.Support;

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

		//Actor can contain more Actors.
		//For light-weight operation, lazy initialization is used.
		LinkedList<IActorCompositePart> actorComposition = null;
		public LinkedList<IActorCompositePart> ActorComposition
		{
			get
			{
				if (actorComposition == null)
				{
					actorComposition = new LinkedList<IActorCompositePart>();
				}
				return actorComposition;
			}
		}

		protected AbstractActor(string name)
		{
			Direction = SpaceDirection.get(SpaceDirection.HorizontalDirection.NONE, SpaceDirection.VerticalDirection.NONE);
			Speed = DEFAULT_SPEED;
			Name = name;
		}

		//Checks whether the X/Y movement is legal
		protected virtual bool CanMoveX(int x)
		{
			return x > 0 && x < Field.Size.Width - Width;
		}
		protected virtual bool CanMoveY(int y)
		{
			return y > 0 && y < Field.Size.Height - Height;
		}

		/// <summary>
		/// Update operation called before each Draw
		/// </summary>
		public virtual void Act()
		{
			//Updating X coordinate if within bounds
			if (Direction.Horizontal == SpaceDirection.HorizontalDirection.LEFT)
			{
				if (CanMoveX(X - Speed))
				{
					X = X - Speed;
				}
			}
			else if (Direction.Horizontal == SpaceDirection.HorizontalDirection.RIGHT)
			{
				if (CanMoveX(X + Speed))
				{
					X = X + Speed;
				}
			}

			//Updating Y coordinate if within bounds
			if (Direction.Vertical == SpaceDirection.VerticalDirection.UP)
			{
				if (CanMoveY(Y - Speed))
				{
					Y = Y - Speed;
				}
			}
			else if (Direction.Vertical == SpaceDirection.VerticalDirection.DOWN)
			{
				if (CanMoveY(Y + Speed))
				{
					Y = Y + Speed;
				}
			}

			//Moving hero's objects together with him.
			//All actors that this actor is composed of always get their coordinates updated to be the same.
			if (ActorComposition != null)
			{
				foreach (IActor actor in ActorComposition)
				{
					actor.X = X;
					actor.Y = Y;
				}
			}
		}

		public void RemoveFromField()
		{
			Field.RemoveActor(this);
		}

		public void AddedToField(IField field)
		{
			Field = field;
			AddedToFieldHook();
		}

		/// <summary>
		/// Hook callback for initialization operations after being connected to a Field
		/// </summary>
		public virtual void AddedToFieldHook()
		{

		}

		/// <summary>
		/// A.K.A HasAlphaOn(x,y).
		/// Lets each actor implement his own collisions using some square calculations.
		/// As a default, implements a square detection.
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <returns>true if the actor can collide on x, y coordinates</returns>
		public virtual bool CollidesOn(int x, int y)
		{
			if(x > X && x < X+Width && y > Y && y < Y+Width)
			{
				return true;
			}
			return false;
		}

		public bool IntersectsActor(IActor actor)
		{
			return actor.CollidesOn(X, Y);
		}

		/// <summary>
		/// Removes the current actor
		/// </summary>
		public virtual void DeleteActor()
		{
			Field.RemoveActor(this);
			Log.i(this, "Removed actor");
		}
	}
}
