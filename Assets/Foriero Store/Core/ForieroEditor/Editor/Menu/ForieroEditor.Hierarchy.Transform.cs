using UnityEngine;
using UnityEditor;
using System.Collections;
using ForieroEngine.Extensions;

namespace ForieroEditor
{
	public static partial class Menu
	{
		[MenuItem ("GameObject/2D Integrator Tool/Center Pivot", false, 1)]
		public static void CenterPivot ()
		{
			foreach (GameObject o in Selection.gameObjects) {
				o.transform.CenterPivot (false, false);
			}
		}

	
		[MenuItem ("GameObject/2D Integrator Tool/Center Pivots", false, 1)]
		public static void CenterPivots ()
		{
			foreach (GameObject o in Selection.gameObjects) {
				o.transform.CenterPivot (true, false);
			}
		}

		[MenuItem ("GameObject/2D Integrator Tool/Center Pivot 2D", false, 1)]
		public static void CenterPivot2D ()
		{
			foreach (GameObject o in Selection.gameObjects) {
				o.transform.CenterPivot (false, true);
			}
		}


		[MenuItem ("GameObject/2D Integrator Tool/Center Pivots 2D", false, 1)]
		public static void CenterPivots2D ()
		{
			foreach (GameObject o in Selection.gameObjects) {
				o.transform.CenterPivot (true, true);
			}
		}
	}
}
