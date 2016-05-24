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

		private float speed;
		public float Speed
		{
			get
			{
				return speed;
			}
			set
			{
				if (value >= 0)
				{
					speed = value;
				}
				else
				{
					speed = 0;
				}
			}
		}

		/// <summary>
		/// Subclass has to implement its choice of collision targets or ignored collision mechanism.
		/// </summary>
		/// <param name="actor">Actor to be collided with.</param>
		/// <returns>true if the actor can cause collision.</returns>
		public abstract bool CollidesWith(IActor actor);

		/// <summary>
		/// Current intersect strategy of the actor.
		/// </summary>
		protected IIntersectStrategy IntersectStrategy
		{
			private get; set;
		}

		/// <summary>
		/// Strategies of the Actor that have the IActStrategy type.
		/// </summary>
		public LinkedList<IActStrategy> ActStrategies
		{
			get; private set;
		}
		/// <summary>
		/// Strategies of the Actor that have the IDrawModificationStrategy type.
		/// </summary>
		public LinkedList<IDrawModificationStrategy> DrawModificationStrategies
		{
			get; private set;
		}

		LinkedList<IActorCompositePart> actorComposition = null;
		/// <summary>
		/// Actor can contain more Actors.
		//	For more light-weight operation, lazy initialization is used.
		/// </summary>
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
			Direction = SpaceDirection.None;
			Speed = DEFAULT_SPEED;

			//Downloads the concrete initialization name choice from the subclass
			Name = name;

			//By default, implements square intersect strategy using the current Animation dimensions
			IntersectStrategy = new SquareIntersect(this);
		}

		/// <summary>
		/// Adds a new strategy to the actor's queue of periodically launched event hooks.
		/// The strategy can be both IActStrategy and IDrawModificationStrategy.
		/// </summary>
		/// <param name="strategy">Strategy to be added to the queue</param>
		protected void AddStrategy(IStrategy strategy)
		{
			IActStrategy act = strategy as IActStrategy;
			IDrawModificationStrategy draw = strategy as IDrawModificationStrategy;

			if (act != null)
			{
				ActStrategies.AddLast(act);
			}

			if (draw != null)
			{
				DrawModificationStrategies.AddLast(draw);
			}
		}

		//Checks whether the X movement is legal
		protected virtual bool CanMoveX(float x)
		{
			//return x > 0 && x < Field.Size.Width - Width;
			//If moving left
			if (x < X)
			{
				return x > 0;
			}
			//If moving right
			else if (x > X)
			{
				return x < Field.Size.Width - Width;
			}
			return false;
		}
		//Checks whether the Y movement is legal
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

		protected virtual bool OutOfFieldBounds()
		{
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

		/// <summary>
		/// Tries to do a movement in the required direction.
		/// Rolls back changes when the movement causes collision.
		/// </summary>
		void MoveHorizontalAndVertical()
		{
			//Stores the old position for a possible later rollback
			Position oldPosition = new Position();
			oldPosition.X = X;
			oldPosition.Y = Y;

			//If already collides, we no longer care about the collision and we just want to prevent getting stuck
			bool alreadyInCollision = RollbackIfCollision(oldPosition);

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

			//If the actor weren't in collision before, we want to rollback a possible collision
			if (!alreadyInCollision)
			{
				float newX = X;
				float newY = Y;

				//We allow a single-directional movement in case it lets the Actor do at least some movement
				if (RollbackIfCollision(oldPosition))
				{
					X = newX;
					if (RollbackIfCollision(oldPosition))
					{
						Y = newY;
						RollbackIfCollision(oldPosition);
					}
				}
			}
		}

		/// <summary>
		/// Returns back to old position if the actor currently collides other actor.
		/// </summary>
		/// <param name="oldPosition">Previous position before the movement started, will be rolled back to on collision.</param>
		/// <returns>true if there is a collision and the position was rolled back.</returns>
		bool RollbackIfCollision(Position oldPosition)
		{
			bool rolledBack = false;

			//Position rollback on collision
			Field.ForEachActor<IActor>
			(
				actor =>
				{
					if
					(
						//Does not collide with self
						actor != this
						&&
						//If there is mutual collision acceptance
						actor.CollidesWith(this) && CollidesWith(actor)
						&&
						//Returns true if positional intersection happened
						IntersectsActor(actor)
					)
					{
						//Rolls back the old position of the current Actor
						X = oldPosition.X;
						Y = oldPosition.Y;

						//Marks the outer function to also return true
						rolledBack = true;
						//Stops iterating over other Actors
						return true;
					}
					return false;
				}
			);

			return rolledBack;
		}

		/// <summary>
		/// Update operation called before each Draw.
		/// </summary>
		public virtual void Act()
		{
			MoveHorizontalAndVertical();

			//Every actor can choose to implement his bounds of lifetime
			if (OutOfFieldBounds())
			{
				DeleteActor();
				Log.i(this, Name + " outside of the map, removed");
				return;
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

			//Acts on all strategies
			foreach (IActStrategy strategy in ActStrategies)
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
			foreach (IActorCompositePart part in ActorComposition)
			{
				Field.RemoveActor(part);
			}
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

		public void AddedToField(IField field)
		{
			DeleteActor();
			Field = field;
			AddedToFieldHook();

			//Adds all parts too
			foreach (IActorCompositePart part in ActorComposition)
			{
				field.AddActor(part);
			}
		}
		public virtual void AddedToFieldHook()
		{
		}

		public virtual void DeleteActor()
		{
			if (Field != null)
			{
				Field.RemoveActor(this);
				Field = null;
				DeleteActorHook();
				Log.i(this, "Removed actor");
			}
		}
		public virtual void DeleteActorHook()
		{
		}

		/// <summary>
		/// AddActor that adds other Actor to the same coordinates on the same Field, centering it in the process.
		/// </summary>
		/// <param name="actor">Actor that is to be added to the same coordinates</param>
		protected void AddActorToSameCoordinates(IActor actor)
		{
			actor.X = X + (float)Width / 2 - (float)actor.Width / 2;
			actor.Y = Y + (float)Height / 2 - (float)actor.Height / 2;
			Field.AddActor(actor);
		}
	}
}
