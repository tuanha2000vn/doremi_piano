using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using ForieroEditor.Extensions;

namespace ForieroEditor
{
    public static partial class Menu
    {
        [MenuItem("Foriero/GitHub/Get Spine Runtime")]
        public static void GetSpineRuntime()
        {
            string spine_unity = "https://github.com/EsotericSoftware/spine-runtimes/trunk/spine-unity/Assets/spine-unity/";
            string spine_src = "https://github.com/EsotericSoftware/spine-runtimes/trunk/spine-csharp/src/";


            GitHub.GetRepositoryFiles(spine_unity, "Assets/Git/Spine/spine-unity");
            GitHub.GetRepositoryFiles(spine_src, "Assets/Git/Spine/src");

            AssetDatabase.Refresh();
        }

        [MenuItem("Foriero/GitHub/Get DOTween")]
        public static void GetDOTween()
        {
            string dotween = "https://github.com/Demigiant/dotween/trunk/_DOTween.Assembly/bin";
            GitHub.GetRepositoryFiles(dotween, "Assets/Demigiant/DOTween");
            AssetDatabase.Refresh();
        }

        [MenuItem("Foriero/GitHub/Get PostProcessing")]
        public static void GetPostProcessing()
        {
            string dotween = "https://github.com/Unity-Technologies/PostProcessing/trunk/PostProcessing";
            GitHub.GetRepositoryFiles(dotween, "Assets/Git/PostProcessing");
            AssetDatabase.Refresh();
        }

        [MenuItem("Foriero/GitHub/Get FrameRecorder")]
        public static void GetGenericFrameRecorder()
        {
            string dotween = "https://github.com/Unity-Technologies/GenericFrameRecorder/trunk/";
            GitHub.GetRepositoryFiles(dotween, "Assets/Git/Recorder/GenericFrameRecorder");

            dotween = "https://github.com/Unity-Technologies/GenericRecorder-FrameCapturerPlugin/trunk/";
            GitHub.GetRepositoryFiles(dotween, "Assets/Git/Recorder/GenericRecorder-FrameCapturerPlugin");

            dotween = "https://github.com/unity3d-jp/FrameCapturer/trunk/FrameCapturer/Assets/UTJ/FrameCapturer";
            GitHub.GetRepositoryFiles(dotween, "Assets/Git/Recorder/FrameCapturer");

            AssetDatabase.Refresh();
        }

        [MenuItem("Foriero/GitHub/Get SpriteShaders")]
        public static void GetSpriteShaders()
        {
            string dotween = "https://github.com/traggett/UnitySpriteShaders/trunk/SpriteShaders";
            GitHub.GetRepositoryFiles(dotween, "Assets/Git/SpriteShaders");
            AssetDatabase.Refresh();
        }

        [MenuItem("Foriero/GitHub/Get CSharpSynth")]
        public static void GetCSharpSynth()
        {
            string dotween = "https://github.com/foriero/csharpsynthunity/trunk/";
            GitHub.GetRepositoryFiles(dotween, "Assets/Git/CSharpSynth");
            AssetDatabase.Refresh();
        }

        [MenuItem("Foriero/GitHub/Get UniRx")]
        public static void GetUniRx()
        {
            string dotween = "https://github.com/neuecc/UniRx/trunk/Assets/Plugins/UniRx/";
            GitHub.GetRepositoryFiles(dotween, "Assets/Git/UniRX");
            AssetDatabase.Refresh();
        }

        [MenuItem("Foriero/GitHub/Get FontAwesome")]
        public static void GetFontAwesome()
        {
            string font = "https://github.com/FortAwesome/Font-Awesome/trunk/fonts/fontawesome-webfont.ttf";
            string variables = "https://github.com/FortAwesome/Font-Awesome/trunk/less/variables.less";

            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "FontAwesome.ttf", SearchOption.AllDirectories);

            string path = "Assets/Git/Fonts";

            if (files.Length != 0)
            {
                path = Path.GetDirectoryName(files[0]).RemoveProjectPath();
            }

            path = path.FixAssetsPath();

            GitHub.GetRepositoryFiles(font, path + "/FontAwesome.ttf");
            GitHub.GetRepositoryFiles(variables, path + "/variables.less");

            AssetDatabase.Refresh();
        }
    }
}