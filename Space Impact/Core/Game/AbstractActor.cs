using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Space_Impact.Core.Game;
using Space_Impact.Support;
using Space_Impact.Core.Game.Player;
using Windows.UI.Xaml.Shapes;
using Space_Impact.Core.Game.IntersectStrategy;
using Space_Impact.Core.Game.PartActor;
using Space_Impact.Core.Game.ActorStrategy;
using Microsoft.Graphics.Canvas;

namespace Space_Impact.Core
{
	public abstract class AbstractActor : AbstractAnimatedObject, IActor
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

		public float Speed
		{
			get; protected set;
		}

		//Current intersect strategy of the actor
		protected IIntersectStrategy IntersectStrategy
		{
			private get; set;
		}

		//Strategies of the Actor
		public LinkedList<IActStrategy> ActStrategies
		{
			get; private set;
		}
		public LinkedList<IDrawModificationStrategy> DrawModificationStrategies
		{
			get; private set;
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
			//Initializes strategy lists
			ActStrategies = new LinkedList<IActStrategy>();
			DrawModificationStrategies = new LinkedList<IDrawModificationStrategy>();

			//Default properties of the Actor
			Direction = SpaceDirection.get(SpaceDirection.HorizontalDirection.NONE, SpaceDirection.VerticalDirection.NONE);
			Speed = DEFAULT_SPEED;

			//Downloads the concrete initialized name choice from the subclass
			Name = name;

			//By default, implements square intersect strategy using the current Animation dimensions
			IntersectStrategy = new SquareIntersect(this);
		}

		//Adds a new strategy to the actor's queue of periodically launched event hooks
		protected void AddStrategy(IStrategy strategy)
		{
			IActStrategy act = strategy as IActStrategy;
			IDrawModificationStrategy draw = strategy as IDrawModificationStrategy;

			if(act!= null)
			{
				ActStrategies.AddLast(act);
			}

			if (draw != null)
			{
				DrawModificationStrategies.AddLast(draw);
			}
		}

		//Checks whether the X/Y movement is legal
		protected virtual bool CanMoveX(float x)
		{
			//return x > 0 && x < Field.Size.Width - Width;
			//If moving left
			if (x < X)
			{
				return x > 0;
			}
			//If moving right
			else if(x > X)
			{
				return x < Field.Size.Width - Width;
			}
			return false;
		}
		protected virtual bool CanMoveY(float y)
		{
			//return y > 0 && y < Field.Size.Height - Height;
			//If moving up
			if (y < Y)
			{
				return y > 0;
			}
			//If moving down
			else if (y > Y)
			{
				return y < Field.Size.Height - Height;
			}
			return false;
		}

		//Current speed in a chosen direction based on angular calculations
		float HorizontalSpeed(float Angle)
		{
			return Speed * (float)Math.Sin((Angle / 180) * Math.PI);
		}
		float VerticalSpeed(float Angle)
		{
			return Speed * (float)Math.Cos((Angle / 180) * Math.PI);
		}

		void MoveHorizontalAndVertical()
		{
			//Updating X coordinate if within bounds
			if (Direction.Horizontal == SpaceDirection.HorizontalDirection.LEFT)
			{
				if (this is IAngle)
				{
					float angle = ((IAngle)this).Angle;
					if (CanMoveX(X + HorizontalSpeed(angle)))
					{
						X = X + HorizontalSpeed(angle);
					}
				}
				else
				{
					if (CanMoveX(X - Speed))
					{
						X = X - Speed;
					}
				}
			}
			else if (Direction.Horizontal == SpaceDirection.HorizontalDirection.RIGHT)
			{
				if (this is IAngle)
				{
					float angle = ((IAngle)this).Angle;
					if (CanMoveX(X + HorizontalSpeed(angle)))
					{
						X = X + HorizontalSpeed(angle);
					}
				}
				else
				{
					if (CanMoveX(X + Speed))
					{
						X = X + Speed;
					}
				}
			}

			//Updating Y coordinate if within bounds
			if (Direction.Vertical == SpaceDirection.VerticalDirection.UP)
			{
				if (this is IAngle)
				{
					float angle = ((IAngle)this).Angle;
					if (CanMoveY(Y - VerticalSpeed(angle)))
					{
						Y = Y - VerticalSpeed(angle);
					}
				}
				else
				{
					if (CanMoveY(Y - Speed))
					{
						Y = Y - Speed;
					}
				}
			}
			else if (Direction.Vertical == SpaceDirection.VerticalDirection.DOWN)
			{
				if (this is IAngle)
				{
					float angle = ((IAngle)this).Angle;
					if (CanMoveY(Y + VerticalSpeed(angle)))
					{
						Y = Y + VerticalSpeed(angle);
					}
				}
				else
				{
					if (CanMoveY(Y + Speed))
					{
						Y = Y + Speed;
					}
				}
			}
		}

		/// <summary>
		/// Update operation called before each Draw
		/// </summary>
		public virtual void Act()
		{
			MoveHorizontalAndVertical();

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

			//Acts on all strategies
			foreach(IActStrategy strategy in ActStrategies)
			{
				strategy.Act();
			}
		}

		protected override void DrawModification(ref ICanvasImage bitmap, CanvasDrawingSession draw)
		{
			base.DrawModification(ref bitmap, draw);

			//Draws all strategies
			foreach (IDrawModificationStrategy strategy in DrawModificationStrategies)
			{
				strategy.DrawModification(ref bitmap, draw);
			}
		}

		public void RemoveFromField()
		{
			Field.RemoveActor(this);

			//Removes all parts too
			foreach(IActorCompositePart part in ActorComposition)
			{
				Field.RemoveActor(part);
			}
		}

		public void AddedToField(IField field)
		{
			Field = field;
			AddedToFieldHook();

			//Adds all parts too
			foreach (IActorCompositePart part in ActorComposition)
			{
				field.AddActor(part);
			}
		}

		/// <summary>
		/// Hook callback for initialization operations after being connected to a Field
		/// </summary>
		public virtual void AddedToFieldHook()
		{
		}

		public bool IntersectsWithin(float x, float width, float y, float height)
		{
			return IntersectStrategy.IntersectsWithin(x, width, y, height);
		}

		/// <summary>
		/// A.K.A HasAlphaOn(x,y).
		/// Lets each actor implement his own collisions using some square calculations.
		/// As a default, implements a square detection.
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <returns>true if the actor can collide on x, y coordinates</returns>
		public bool IntersectsOn(float x, float y)
		{
			return IntersectStrategy.IntersectsOn(x, y);
		}

		public bool IntersectsActor(IActor actor)
		{
			return IntersectStrategy.IntersectsActor(actor);
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
