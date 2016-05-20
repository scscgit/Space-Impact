using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Core
{
	public interface IField
	{
		void AddActor(IActor actor);
		void RemoveActor(IActor actor);
	}
}
