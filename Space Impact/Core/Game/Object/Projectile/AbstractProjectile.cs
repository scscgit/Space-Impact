using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Space_Impact.Core.Game.ActorStrategy;

namespace Space_Impact.Core.Game.Object.Weapon
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

		public float Angle
		{
			get; set;
		}

		/// <summary>
		/// Projectile that moves in a certain direction at a certain angle.
		/// </summary>
		/// <param name="name">name of the projectile object</param>
		/// <param name="direction">direction to which the projectile can move</param>
		/// <param name="angle">angle in degrees of the movement</param>
		protected AbstractProjectile(string name, SpaceDirection direction, float angle) : base(name)
		{
			Direction = direction;
			Angle = angle;
		}

		protected override void DrawAbstractModification(ref ICanvasImage bitmap, CanvasDrawingSession draw)
		{
			base.DrawAbstractModification(ref bitmap, draw);
			
			Rotation.DrawModification(ref bitmap, draw, Rotation.DegreeToRadians(Angle));
		}
	}
}
