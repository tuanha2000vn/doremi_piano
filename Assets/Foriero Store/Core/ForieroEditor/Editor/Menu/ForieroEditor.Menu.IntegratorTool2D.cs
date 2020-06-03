using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace ForieroEditor
{
	public static partial class Menu
	{
		[MenuItem ("Assets/2D Integrator Tool/Recreate")]
		public static void IllustratorRecreate ()
		{
			IntegratorTool2DEditor.RecreateScene.GenerateHierarchy (AssetDatabase.GetAssetPath (Selection.objects [0]), "");
		}
	}
}