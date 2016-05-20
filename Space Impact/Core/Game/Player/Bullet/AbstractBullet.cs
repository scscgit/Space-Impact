using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core.Game.Player.Bullet
{
	public abstract class AbstractBullet : AbstractActor, IBullet
	{
		protected IPlayer Player
		{
			get; set;
		}

		public AbstractBullet(string name, IPlayer player, SpaceDirection direction) : base(name)
		{
			Player = player;
			Direction = direction;

			//Moves to the player's coordinates
			Position position = player.BulletFocusPosition;
			X = position.X;
			Y = position.Y;
		}

		//Removes all movement limitations, instead verifies in the Act whether the object needs to be destroyed
		protected override bool CanMoveX(int x)
		{
			return true;
		}
		protected override bool CanMoveY(int y)
		{
			return true;
		}

		public override void Act()
		{
			base.Act();

			if (X < -Width || X > Field.Size.Width || Y < -Height || Y > Field.Size.Height)
			{
				DeleteActor();
			}
		}
	}
}
