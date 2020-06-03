using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
//using UniRx;
using System;

namespace ForieroEditor
{
    public static partial class Menu
    {
        private static readonly Uri kPackageUri = new Uri("https://public-cdn.cloud.unity3d.com/UnityEngine.Cloud.Purchasing.unitypackage");

        // Works fine //
        [MenuItem("Foriero/GitHub/Manual IAP Download (UniRx)")]
        static public void DoDownloadUniRx()
        {
            //ObservableWWW.GetAndGetBytes(kPackageUri.ToString()).Subscribe(bytes =>
            //{
            //    SaveAndImport(bytes);
            //}, ex => Debug.LogException(ex));
            EditorCoroutineStart.StartCoroutine(Download());
        }


        static IEnumerator Download()
        {
            WWW www = new WWW(kPackageUri.ToString());

            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                SaveAndImport(www.bytes);
            }
            else
            {
                Debug.LogError(www.error);
            }
        }

        static void SaveAndImport(byte[] bytes)
        {
            var location = FileUtil.GetUniqueTempPathInProject();
            // Extension is required for correct Windows import.
            location = Path.ChangeExtension(location, ".unitypackage");

            File.WriteAllBytes(location, bytes);

            AssetDatabase.ImportPackage(location, false);
        }
    }
}