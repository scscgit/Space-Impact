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

		//Ships and their thrusts
		public static string[] SHIP1_BASE = { "ship1_base" };
		public static string[] SHIP1_THRUST = { "ship1_thrust" };
		public static string[] SHIP2_BASE = { "ship2_base" };
		public static string[] SHIP2_THRUST = { "ship2_thrust" };
		public static string[] SHIP3_BASE = { "ship3_base" };
		public static string[] SHIP3_THRUST = { "ship3_thrust" };
		public static string[] SHIP4_BASE = { "ship4_base" };
		public static string[] SHIP4_THRUST = { "ship4_thrust" };
		public static string[] SHIP5_BASE = { "ship5_base" };
		public static string[] SHIP5_THRUST = { "ship5_thrust" };
		public static string[] SHIP6_BASE = { "ship6_base" };
		public static string[] SHIP6_THRUST = { "ship6_thrust" };
		public static string[] SHIP7_BASE = { "ship7_base" };
		public static string[] SHIP7_THRUST = { "ship7_thrust" };
		public static string[] SHIP8_BASE = { "ship8_base" };
		public static string[] SHIP8_THRUST = { "ship8_thrust" };
		public static string[] SHIP9_BASE = { "ship9_base" };
		public static string[] SHIP9_THRUST = { "ship9_thrust" };
		public static string[] SHIP10_BASE = { "ship10_base" };
		public static string[] SHIP10_THRUST = { "ship10_thrust" };
		public static string[] SHIP11_BASE = { "ship11_base" };
		public static string[] SHIP11_THRUST = { "ship11_thrust" };
		public static string[] SHIP12_BASE = { "ship12_base" };
		public static string[] SHIP12_THRUST = { "ship12_thrust" };

		//Bombs maybe?
		public static string[] DOOMDAY = { "doomday_1", "doomday_2", "doomday_3" };
	
		//Bullets
		public static string[] FIRE = { "fire_1", "fire_2" };

		//Loading all textures
		public async Task CreateResourcesAsync(CanvasAnimatedControl sender)
		{
			//Set of all textureSets to be loaded
			string[][] textureSets =
			{
				//Ships and their thrusts
				SHIP1_BASE, SHIP1_THRUST,
				SHIP2_BASE, SHIP2_THRUST,
				SHIP3_BASE, SHIP3_THRUST,
				SHIP4_BASE, SHIP4_THRUST,
				SHIP5_BASE, SHIP5_THRUST,
				SHIP6_BASE, SHIP6_THRUST,
				SHIP7_BASE, SHIP7_THRUST,
				SHIP8_BASE, SHIP8_THRUST,
				SHIP9_BASE, SHIP9_THRUST,
				SHIP10_BASE, SHIP10_THRUST,
				SHIP11_BASE, SHIP11_THRUST,
				SHIP12_BASE, SHIP12_THRUST,

				//Bombs maybe?
				DOOMDAY,

				//Bullets
				FIRE
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
				try
				{
					bitmapSet[i] = await CanvasBitmap.LoadAsync(sender, TextureToString(textureSet[i]));
				}
				catch(System.IO.FileNotFoundException)
				{
					Log.e(this,"Problem loading texture from assets: "+textureSet[i]);
				}
				Log.i(this, "Loading texture "+textureSet[i]);
			}
			this.bitmapDictionary.Add(textureSet, bitmapSet);
		}
	}
}
