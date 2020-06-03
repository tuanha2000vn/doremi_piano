using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MIDIEditorSettings))]
[CanEditMultipleObjects()]
public class MIDIEditorSettingsInspector : Editor
{
    MIDIEditorSettings o;

    public void OnEnable()
    {
        o = target as MIDIEditorSettings;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (GUILayout.Button("Open MIDI Settings"))
        {
            EditorGUIUtility.PingObject(MIDISettings.instance);
            Selection.objects = new Object[1] { MIDISettings.instance };
            Selection.activeObject = MIDISettings.instance;
        }

        DrawDefaultInspector();

        EditorGUI.BeginChangeCheck();

        if (GUI.changed || EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(o);
        }
    }
}