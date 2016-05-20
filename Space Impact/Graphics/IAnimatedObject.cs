using System.Threading.Tasks;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Space_Impact.Core;
using Microsoft.Graphics.Canvas;

namespace Space_Impact.Graphics
{
	public interface IAnimatedObject : IPlacedOnField
	{
		int Frame { get; }
		int AnimationSpeed { set; }

		void setAnimation(string[] textures);
		
		void Draw(CanvasDrawingSession draw);
	}
}