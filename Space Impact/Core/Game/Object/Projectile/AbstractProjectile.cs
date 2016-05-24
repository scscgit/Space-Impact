using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Space_Impact.Core.Game.ActorStrategy;
using Space_Impact.Support;
using Space_Impact.Core.Game.Character;
using Space_Impact.Graphics;
using Space_Impact.Core.Game.ActorStrategy.Rotation;

namespace Space_Impact.Core.Game.Object.Projectile
{
	public abstract class AbstractProjectile : AbstractObject, IProjectile
	{
		//Constants
		const int DEFAULT_DAMAGE = 1;

		//Properties
		int damage = DEFAULT_DAMAGE;
		public int Damage
		{
			get
			{
				return this.damage;
			}
			protected set
			{
				if (value > 0)
				{
					this.damage = value;
				}
				else
				{
					this.damage = 0;
				}
			}
		}

		//Projectile reimplements functionality of the Direction field, it would be maybe useful to hide the Direction field for changes.
		//Angle is in degrees.
		private float angle;
		public float Angle
		{
			get
			{
				return this.angle;
			}
			set
			{
				this.angle = value;
				Direction = SpaceDirection.GetFromAngle(angle);
			}
		}

		/// <summary>
		/// Owner of the projectile.
		/// </summary>
		protected ICharacter Character
		{
			get; set;
		}

		/// <summary>
		/// Starting position of the projectile.
		/// </summary>
		private Position StartPosition;

		/// <summary>
		/// Projectile that moves in a certain direction at a certain angle.
		/// </summary>
		/// <param name="name">Name of the projectile object</param>
		/// <param name="character">Owner of the projectile</param>
		/// <param name="position">Starting position of the projectile</param>
		/// <param name="angle">Angle of the movement in degrees</param>
		protected AbstractProjectile(string name, ICharacter character, Position position, float angle) : base(name)
		{
			Character = character;
			StartPosition = position;
			Angle = angle;
		}

		protected override void DrawAbstractModification(ref ICanvasImage bitmap, CanvasDrawingSession draw)
		{
			base.DrawAbstractModification(ref bitmap, draw);

			AbstractRotation.DrawModification(ref bitmap, draw, AbstractRotation.DegreesToRadians(Angle));
		}

		protected override void OnAnimationSet()
		{
			base.OnAnimationSet();

			//Moves to the starting coordinates, then centers itself based on a current new Animation size
			X = StartPosition.X - (float)Width / 2;
			Y = StartPosition.Y - (float)Height / 2;
		}
	}
}
