using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ForieroEngine.Extensions;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace ForieroEngine
{
    public static class FResources
    {
        public static Dictionary<string, object> instances = new Dictionary<string, object>();

        public static T Instance<T>(string name, string assetPath = "") where T : ScriptableObject
        {
            if (!string.IsNullOrEmpty(assetPath))
            {
                assetPath = assetPath.StartsWith("/") ? assetPath.Remove(0, 1) : assetPath;
                assetPath = assetPath.EndsWith("/") ? assetPath : assetPath + "/";
            }

            var path = assetPath + name;

            T instance = default(T);

            if (instances.ContainsKey(path))
            {
                //Debug.Log ("FResources Loading Cached : " + path);
                instance = (T)instances[path];

            }
            else
            {
                //Debug.Log ("FResources Loading : " + path);
                instance = Resources.Load<T>(path);
                instances.Add(path, instance);
            }

            if (instance == null)
            {
                if (Application.isPlaying)
                {
                    Debug.LogError(name + " did not existed, creating one in memory!");
                }

                instance = ScriptableObject.CreateInstance<T>();

#if UNITY_EDITOR
                var resourcesPath = "Assets/Resources/" + path + ".asset";
                //Debug.Log ("FResource Path : " + resourcesPath);

                var resourceDirectory = Application.dataPath + "/Resources/" + assetPath;
                //Debug.Log ("FResource Directory : " + resourceDirectory);

                if (!Directory.Exists(resourceDirectory))
                {
                    System.IO.Directory.CreateDirectory(resourceDirectory);
                }

                AssetDatabase.CreateAsset(instance, resourcesPath);

                AssetDatabase.SaveAssets();
#endif
            }

            return instance;
        }

#if UNITY_EDITOR
        public static T EditorInstance<T>(string name, string assetPath = "Assets/Editor/", bool newInstance = false) where T : ScriptableObject
        {
            if (!string.IsNullOrEmpty(assetPath))
            {
                assetPath = assetPath.StartsWith("/") ? assetPath.Remove(0, 1) : assetPath;
                assetPath = assetPath.EndsWith("/") ? assetPath : assetPath + "/";
            }

            var path = assetPath + name + ".asset";
            //Debug.Log ("FResources Loading : " + path);

            T instance = AssetDatabase.LoadAssetAtPath<T>(path);

            if (newInstance || instance == null)
            {
                if (Application.isPlaying)
                {
                    Debug.LogError(name + " did not existed, creating one in memory!");
                }

                instance = ScriptableObject.CreateInstance<T>();

                var resourcesPath = path;
                //Debug.Log ("FResource Path : " + resourcesPath);

                var resourceDirectory = Path.Combine(Directory.GetCurrentDirectory(), assetPath.FixOSPath());
                //Debug.Log ("FResource Directory : " + resourceDirectory);

                if (!Directory.Exists(resourceDirectory))
                {
                    System.IO.Directory.CreateDirectory(resourceDirectory);
                }

                AssetDatabase.CreateAsset(instance, AssetDatabase.GenerateUniqueAssetPath(resourcesPath));

                AssetDatabase.SaveAssets();
            }

            return instance;
        }
#endif
    }
}