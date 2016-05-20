using Space_Impact.Core.Game.Object.Weapon;
using Space_Impact.Graphics;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Player.Bullet
{
	public abstract class AbstractBullet : AbstractProjectile, IBullet
	{
		protected IPlayer Player
		{
			get; set;
		}

		protected AbstractBullet(string name, IPlayer player, float angle) : base(name, angle)
		{
			Player = player;
		}

		protected override void OnAnimationSet()
		{
			base.OnAnimationSet();

			//Moves to the player's coordinates for a bullet, then centers itself based on a current new Animation size
			Position centerPosition = Player.BulletFocusPosition;
			X = centerPosition.X - (float)Width / 2;
			Y = centerPosition.Y - (float)Height /2;
			Log.d(this, centerPosition.X.ToString() + " " + (Width).ToString());
		}

		//Removes all movement limitations, instead verifies in the Act whether the object needs to be destroyed
		protected override bool CanMoveX(float x)
		{
			return true;
		}
		protected override bool CanMoveY(float y)
		{
			return true;
		}

		bool OutOfFieldBounds()
		{
			return X < -Width || X > Field.Size.Width || Y < -Height || Y > Field.Size.Height;
		}

		public override void Act()
		{
			base.Act();

			if (OutOfFieldBounds())
			{
				DeleteActor();
				Log.d(this, "Bullet outside of the map, removed");
			}
		}
	}
}
