using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Space_Impact.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Impact.Graphics
{
	public class TextureSetLoader
	{
		//List of all pre-defined texture sets
		public static string[] SHIP1_BASE = { "ship1_base" };
		public static string[] SHIP1_THRUST = { "ship1_thrust" };

		//Loading all textures
		public async Task CreateResourcesAsync(CanvasAnimatedControl sender)
		{
			//Set of all textureSets to be loaded
			string[][] textureSets =
			{
				SHIP1_BASE, SHIP1_THRUST
			};

			//Loads each textureSet
			foreach (string[] textureSet in textureSets)
			{
				await load(sender, textureSet);
			}
		}

		//Resets all loaded textures by destroying the current TextureSetLoader Instance
		public static void DeleteInstance()
		{
			textureSetLoader = null;
		}

		//Singleton
		public static TextureSetLoader Instance
		{
			get
			{
				if (textureSetLoader == null)
				{
					textureSetLoader = new TextureSetLoader();
				}
				return textureSetLoader;
			}
		}
		private static TextureSetLoader textureSetLoader = null;
		TextureSetLoader()
		{
		}

		//Mapping texture sets to canvas bitmap sets, main accessor point for bitmaps
		private IDictionary<string[], CanvasBitmap[]> bitmapDictionary = new Dictionary<string[], CanvasBitmap[]>();
		public CanvasBitmap[] this[string[] textureSet]
		{
			get
			{
				return bitmapDictionary[textureSet];
			}
		}

		//Internal conversion of texture format to a full file name
		private string TextureToString(string texture)
		{
			return "Assets/" + texture + ".png";
		}

		//Loads a bitmap representing each texture and adds them to the Dictionary
		async Task load(CanvasAnimatedControl sender, string[] textureSet)
		{
			int length = textureSet.Length;
			CanvasBitmap[] bitmapSet = new CanvasBitmap[length];
			for (int i = 0; i < length; i++)
			{
				bitmapSet[i] = await CanvasBitmap.LoadAsync(sender, TextureToString(textureSet[i]));
				Log.i(this, "Loading texture "+textureSet[i]);
			}
			this.bitmapDictionary.Add(textureSet, bitmapSet);
		}
	}
}
