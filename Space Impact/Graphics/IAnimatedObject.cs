using System.Threading.Tasks;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Space_Impact.Core;
using Microsoft.Graphics.Canvas;

namespace Space_Impact.Graphics
{
	public interface IAnimatedObject : IPlacedInSpace, IDrawable
	{
		int Frame { get; }
		int AnimationSpeed { set; }
		string[] Animation { set; }
	}
}