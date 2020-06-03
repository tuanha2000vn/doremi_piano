using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
using System.Text.RegularExpressions;

using Image = UnityEngine.UI.Image;

namespace ForieroEditor
{
    public static class ScriptableObjects
    {
        public static void CreateScriptableObject<T>() where T : ScriptableObject
        {
            T scriptable_object = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (System.IO.Path.GetExtension(path) != "")
            {
                path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string asset_and_path_name = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).Name + ".asset");

            AssetDatabase.CreateAsset(scriptable_object, asset_and_path_name);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = scriptable_object;
        }
    }
}
