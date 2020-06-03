using System;
using UnityEngine;
using UnityEditor;
using ForieroEditor.Extensions;

public abstract class ScriptableObjectSettingsWindow<T> : EditorWindow where T : ScriptableObject
{
    T _settings;
    public T settings
    {
        get
        {
            return _settings;
        }
        set
        {
            _settings = value;
            InitObject(value);
        }
    }

    SerializedObject _serializedObject;
    public SerializedObject serializedObject
    {
        get
        {
            return _serializedObject;
        }
        set
        {
            _serializedObject = value;
            InitSerializedObject(value);
        }
    }

    protected string[] settingsGUIDs = new string[0];
    protected string[] settingsNames = new string[0];

    protected string selectedSettingsGUID = "";
    protected int selectedSettingsIndex = -1;
    protected Color backgroundColor;

    public abstract void InitSerializedObject(SerializedObject serializedObject);
    public abstract void InitObject(T o);

    public void InitSettings(T settings)
    {
        selectedSettingsGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(settings));
        RefreshSettings();
    }

    public void InitSettings(string guid)
    {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(assetPath))
        {
            selectedSettingsGUID = "";
            selectedSettingsIndex = -1;
            RefreshSettings();
        }
        else
        {
            settings = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (settings == null)
            {
                selectedSettingsGUID = "";
                selectedSettingsIndex = -1;
                RefreshSettings();
            }
            else
            {
                serializedObject = new SerializedObject(settings);
                selectedSettingsGUID = guid;
            }
        }
    }

    public bool OnSettingsGUI()
    {
        bool result = true;

        backgroundColor = GUI.backgroundColor;

        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));

        EditorGUILayout.BeginHorizontal();

        GUILayout.Space(4);

        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
        {
            RefreshSettings();
        }

        GUILayout.Label(settings == null ? "Please select settings or create new one ->" : settings.name, EditorStyles.toolbarButton);

        if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(20)))
        {
            string path = "";
            path = EditorUtility.SaveFilePanel("Save " + typeof(T).Name, Application.dataPath, typeof(T).Name, "asset");

            if (!string.IsNullOrEmpty(path))
            {
                var instance = ScriptableObject.CreateInstance<T>();

                AssetDatabase.CreateAsset(instance, path.RemoveProjectPath().FixAssetsPath());

                AssetDatabase.SaveAssets();

                EditorGUIUtility.PingObject(instance);

                selectedSettingsGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(instance));

                RefreshSettings();
            }
        }

        GUI.changed = false;
        selectedSettingsIndex = EditorGUILayout.Popup(selectedSettingsIndex, settingsNames);
        if (GUI.changed)
        {
            selectedSettingsGUID = settingsGUIDs[selectedSettingsIndex];
            InitSettings(selectedSettingsGUID);
        }

        GUI.backgroundColor = Color.red;

        GUI.enabled = selectedSettingsIndex >= 0;

        if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(20)))
        {
            if (EditorUtility.DisplayDialog("Delete settings", "Delete : " + settingsNames[selectedSettingsIndex], "Yes", "No"))
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(settingsGUIDs[selectedSettingsIndex]));

                selectedSettingsGUID = "";
                selectedSettingsIndex = -1;
                RefreshSettings();
            }
        }

        GUI.enabled = true;

        GUI.backgroundColor = backgroundColor;

        GUILayout.Space(4);

        EditorGUILayout.EndHorizontal();

        if (serializedObject == null)
        {
            if (selectedSettingsIndex >= 0 && selectedSettingsIndex < settingsNames.Length)
            {
                selectedSettingsGUID = settingsGUIDs[selectedSettingsIndex];
                InitSettings(selectedSettingsGUID);
            }
            else
            {
                result = false;
            }
        }

        if (settings == null)
        {
            serializedObject = null;
            result = false;
        }

        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));

        return result;
    }

    public void RefreshSettings()
    {
        selectedSettingsIndex = -1;

        settingsGUIDs = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        settingsNames = new string[settingsGUIDs.Length];
        for (int i = 0; i < settingsGUIDs.Length; i++)
        {
            settingsNames[i] = AssetDatabase.GUIDToAssetPath(settingsGUIDs[i]).Replace("/", "\\");
            if (selectedSettingsGUID == settingsGUIDs[i])
            {
                selectedSettingsIndex = i;
                InitSettings(settingsGUIDs[i]);
            }
        }
    }

    public void OnSettingsEnable()
    {
        if (serializedObject == null)
        {
            return;
        }


        if (settings == null)
        {
            serializedObject = null;
            return;
        }

        RefreshSettings();
    }

    public void OnSettingsDestroy()
    {
        settings = null;
        serializedObject = null;
    }
}
