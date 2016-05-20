using Space_Impact.Core.Game;
using Space_Impact.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core
{
	public interface IActor : IAnimatedObject
	{
		IField Field
		{
			get;
		}

		string Name
		{
			set; get;
		}

		SpaceDirection Direction
		{
			get; set;
		}

		int Speed
		{
			get; set;
		}

		void Act();
		void AddedToField(IField field);
		void AddedToFieldHook();
	}
}
