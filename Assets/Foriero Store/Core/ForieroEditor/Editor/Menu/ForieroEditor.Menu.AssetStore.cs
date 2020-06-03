using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.IO;

namespace ForieroEditor
{
	public static class MenuExtensions
	{
		public static string ToStoreId (this int id)
		{
			if (id == 0) {
				return "/search/query=publisher:352";
			} else {
				return "/content/" + id.ToString ();
			}
		}
	}


	public static partial class Menu
	{
		//-------//
		static int doTweenId = 27676;
		static int easyTouch = 3322;
		static int playMaker = 368;
		//-------//

		static int gunsVol1 = 54981;
		//static int gunsVol2 = 0;
		static int treesVol1 = 51071;
		static int treesVol2 = 52809;
		static int flowersVol1 = 52810;
		static int platfomerVol1 = 52817;
		static int parallaxVol1 = 53230;
		static int gemsVol1 = 28764;
		static int gemsVol2 = 52813;
		static int childrenVol1 = 52812;
		static int animalsVol1 = 52811;
		static int fishesVol1 = 53231;
		static int villageVol1 = 53101;
		static int cartoonProGUI = 28567;
		static int woodProGUI = 28496;
		static int skiesVol1 = 52818;
		static int backgroundsVol1 = 51075;
		static int backgroundsVol2 = 53785;

		static int assemblyBuilder = 3212;
		static int hardLinks = 55471;
		static int illustratorTool = 25491;
		static int macAppStore = 54970;
		static int storeShots = 54971;

		[MenuItem ("Foriero/Asset Store/3rd/Easy Touch")]
		public static void OpenEasyTouch ()
		{
			AssetStore.Open (easyTouch.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/3rd/DOTween")]
		public static void OpenDOTween ()
		{
			AssetStore.Open (doTweenId.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/3rd/Playmaker")]
		public static void OpenPlaymaker ()
		{
			AssetStore.Open (playMaker.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Packages/Animals/Fishes 2D Vol.1")]
		public static void OpenFishesVol1 ()
		{
			AssetStore.Open (fishesVol1.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Packages/Backgrounds/Backgrounds 2D Vol.1")]
		public static void OpenBackgroundsVol1 ()
		{
			AssetStore.Open (backgroundsVol1.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Packages/Backgrounds/Backgrounds 2D Vol.2")]
		public static void OpenBackgroundsVol2 ()
		{
			AssetStore.Open (backgroundsVol2.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Packages/Buildings/Village 2D Vol.1")]
		public static void OpenVillageVol1 ()
		{
			AssetStore.Open (villageVol1.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Packages/Characters/Children 2D Vol.1")]
		public static void OpenChildrenVol1 ()
		{
			AssetStore.Open (childrenVol1.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Packages/Characters/Animals 2D Vol.1")]
		public static void OpenAnimalsVol1 ()
		{
			AssetStore.Open (animalsVol1.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Packages/Gems/Gems 2D Vol.1")]
		public static void OpenGemsVol1 ()
		{
			AssetStore.Open (gemsVol1.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Packages/Gems/Gems 2D Vol.2")]
		public static void OpenGemsVol2 ()
		{
			AssetStore.Open (gemsVol2.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Packages/GUI/Cartoon PRO GUI")]
		public static void OpenCartoonProGUI ()
		{
			AssetStore.Open (cartoonProGUI.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Packages/GUI/Wood PRO GUI")]
		public static void OpenWoodProGUI ()
		{
			AssetStore.Open (woodProGUI.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Packages/Parallaxes/Parallax 2D Vol.1")]
		public static void OpenParallaxVol1 ()
		{
			AssetStore.Open (parallaxVol1.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Packages/Plants/Flowers 2D Vol.1")]
		public static void OpenFlowersVol1 ()
		{
			AssetStore.Open (flowersVol1.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Packages/Plants/Trees 2D Vol.1")]
		public static void OpenTreesVol1 ()
		{
			AssetStore.Open (treesVol1.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Packages/Plants/Trees 2D Vol.2")]
		public static void OpenTreesVol2 ()
		{
			AssetStore.Open (treesVol2.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Packages/Platformers/Platformer 2D Vol.1")]
		public static void OpenPlatformerVol1 ()
		{
			AssetStore.Open (platfomerVol1.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Packages/Skies/Skies 2D Vol.1")]
		public static void OpenSkiesVol1 ()
		{
			AssetStore.Open (skiesVol1.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Packages/Weapons/Guns 2D Vol.1")]
		public static void OpenGunsVol1 ()
		{
			AssetStore.Open (gunsVol1.ToStoreId ());
		
		}

		//		[MenuItem ("Foriero/Asset Store/Packages/Weapons/Guns 2D Vol.2")]
		//		public static void OpenGunsVol2 ()
		//		{
		//			AssetStore.Open (gunsVol2.ToStoreId ());
		//		}

		[MenuItem ("Foriero/Asset Store/Tools/Assembly Builder")]
		public static void OpenAssemblyBuilder ()
		{
			AssetStore.Open (assemblyBuilder.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Tools/Hard Links")]
		public static void OpenHardLinks ()
		{
			AssetStore.Open (hardLinks.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Tools/Illustrator Tool")]
		public static void OpenIllustratorTool ()
		{
			AssetStore.Open (illustratorTool.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Tools/Mac App Store - Signed")]
		public static void OpenMacAppStore ()
		{
			AssetStore.Open (macAppStore.ToStoreId ());
		}

		[MenuItem ("Foriero/Asset Store/Tools/Store Shots")]
		public static void OpenStoreShots ()
		{
			AssetStore.Open (storeShots.ToStoreId ());
		}
	}
}