using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace ForieroEditor
{
	public static partial class Menu
	{
		[MenuItem ("Foriero/Sprites/Swap/To @1x")]
		public static void SwapSprites1x ()
		{
			Sprites.SwapTo ("@1x");
		}

		[MenuItem ("Foriero/Sprites/Swap/To @2x")]
		public static void SwapSprites2x ()
		{
			Sprites.SwapTo ("@2x");
		}

		[MenuItem ("Foriero/Sprites/Swap/To @4x")]
		public static void SwapSprites4x ()
		{
			Sprites.SwapTo ("@4x");
		}
	}
}