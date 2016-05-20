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
	public sealed class TextureSetLoader
	{
		//List of all pre-defined texture sets

		//Backgrounds
		public static string[] BG1 = { "Background/background_1.jpg" };
		public static string[] BG_CRATER_PLANET = { "Background/crater_planet.jpg" };
		public static string[] BG_MESSIER = { "Background/messier_101.jpg" };
		public static string[] BG_ROCKET_AND_PLANET = { "Background/rocket_and_planet.jpg" };
		public static string[] BG_STAR_CLUSTERS = { "Background/star_clusters_74052.jpg" };

		//Ships and their thrusts
		public static string[] SHIP1_BASE = { "ship1_base.png" };
		public static string[] SHIP1_THRUST = { "ship1_thrust.png" };
		public static string[] SHIP2_BASE = { "ship2_base.png" };
		public static string[] SHIP2_THRUST = { "ship2_thrust.png" };
		public static string[] SHIP3_BASE = { "ship3_base.png" };
		public static string[] SHIP3_THRUST = { "ship3_thrust.png" };
		public static string[] SHIP4_BASE = { "ship4_base.png" };
		public static string[] SHIP4_THRUST = { "ship4_thrust.png" };
		public static string[] SHIP5_BASE = { "ship5_base.png" };
		public static string[] SHIP5_THRUST = { "ship5_thrust.png" };
		public static string[] SHIP6_BASE = { "ship6_base.png" };
		public static string[] SHIP6_THRUST = { "ship6_thrust.png" };
		public static string[] SHIP7_BASE = { "ship7_base.png" };
		public static string[] SHIP7_THRUST = { "ship7_thrust.png" };
		public static string[] SHIP8_BASE = { "ship8_base.png" };
		public static string[] SHIP8_THRUST = { "ship8_thrust.png" };
		public static string[] SHIP9_BASE = { "ship9_base.png" };
		public static string[] SHIP9_THRUST = { "ship9_thrust.png" };
		public static string[] SHIP10_BASE = { "ship10_base.png" };
		public static string[] SHIP10_THRUST = { "ship10_thrust.png" };
		public static string[] SHIP11_BASE = { "ship11_base.png" };
		public static string[] SHIP11_THRUST = { "ship11_thrust.png" };
		public static string[] SHIP12_BASE = { "ship12_base.png" };
		public static string[] SHIP12_THRUST = { "ship12_thrust.png" };

		//Bombs maybe?
		public static string[] DOOMDAY = { "doomday_1.png", "doomday_2.png", "doomday_3.png" };

		//Bullets
		public static string[] FIRE = { "fire_1.png", "fire_2.png" };

		//Loading all textures
		public delegate void IncreaseLoadedPercentageDelegate(float percent);
		public async Task CreateResourcesAsync(CanvasAnimatedControl sender, IncreaseLoadedPercentageDelegate increaseLoadedPercentage)
		{
			//Set of all textureSets to be loaded
			string[][] textureSets =
			{
				//Backgrounds
				BG1, BG_CRATER_PLANET, BG_MESSIER,
				BG_ROCKET_AND_PLANET, BG_STAR_CLUSTERS,

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
				
				//Increases progress where applicable
				if (increaseLoadedPercentage != null)
				{
					increaseLoadedPercentage((float)100 / textureSets.Length);
				}
			}
		}

		//Resets all loaded textures by destroying the current TextureSetLoader Instance
		public static void DeleteInstance()
		{
			textureSetLoader = null;
		}

		private static object ConstructorLock = new object();

		//Singleton
		private static TextureSetLoader textureSetLoader = null;
		public static TextureSetLoader Instance
		{
			get
			{
				lock (ConstructorLock)
				{
					if (textureSetLoader == null)
					{
						textureSetLoader = new TextureSetLoader();
					}
					return textureSetLoader;
				}
			}
		}
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
			return "Assets/" + texture;
		}

		//Loads a bitmap representing each texture and adds them to the Dictionary
		async Task load(CanvasAnimatedControl sender, string[] textureSet)
		{
			int length = textureSet.Length;
			CanvasBitmap[] bitmapSet = new CanvasBitmap[length];
			for (int i = 0; i < length; i++)
			{
				Log.i(this, "Loading texture " + textureSet[i]);
				try
				{
					bitmapSet[i] = await CanvasBitmap.LoadAsync(sender, TextureToString(textureSet[i]));
				}
				catch (System.IO.FileNotFoundException)
				{
					Log.e(this, "Problem loading texture from assets: " + textureSet[i]);
				}
			}
			this.bitmapDictionary.Add(textureSet, bitmapSet);
		}
	}
}
