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

		void Act();
		void AddedToField(IField field);
		void AddedToFieldHook();
	}
}
