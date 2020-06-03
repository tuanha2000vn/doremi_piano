using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;
using System.IO;
using ForieroEditor.Extensions;

public class MIDIUnifiedPreprocessBuildPlayer : IPreprocessBuild
{
    public static bool called = false;

    public static readonly string resourcesPath = "Assets/Resources/soundfont.sf2.bytes";
    public static readonly string lastCopiedSoundFontGUID = "LAST_COPIED_SOUNDFONT_GUID";

    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        Apply();
    }

    [PostProcessScene]
    public static void OnPreprocessScene()
    {
        Apply();
    }

    static void Apply()
    {
        if (called) return;
#if UNITY_ANDROID || UNITY_WSA
        if (MIDISettings.instance.GetPlatformSettings().GetSynthEnum() == Synth.SynthEnum.CSharp || MIDISettings.instance.GetPlatformSettings().GetSynthEnum() == Synth.SynthEnum.Bass24)
#else
        if (MIDISettings.instance.GetPlatformSettings().GetSynthEnum() == Synth.SynthEnum.CSharp)
#endif
        {
            called = true;

            Debug.Log(MIDIEditorSettings.instance.GetPlatformSoundFontAssetPath());
            Debug.Log(MIDIEditorSettings.instance.GetPlatformSoundFontFullPath());

            DuplicateAndCopyToResources(MIDIEditorSettings.instance.GetPlatformSoundFont());
        }
        else
        {
            AssetDatabase.DeleteAsset(resourcesPath);
        }
    }

    static void DuplicateAndCopyToResources(TextAsset soundFont)
    {
        if (soundFont)
        {
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(soundFont));

            if (!File.Exists(resourcesPath.GetFullPathFromAssetPath()) || guid != EditorPrefs.GetString(lastCopiedSoundFontGUID, ""))
            {
                EditorPrefs.SetString(lastCopiedSoundFontGUID, guid);
                AssetDatabase.DeleteAsset(resourcesPath);
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(soundFont), resourcesPath);
                AssetDatabase.SaveAssets();
            }
        }
    }
}