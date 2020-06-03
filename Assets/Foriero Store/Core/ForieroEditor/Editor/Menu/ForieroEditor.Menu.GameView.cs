using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ForieroEditor
{
    public static partial class Menu
    {
        [MenuItem("Foriero/GameView/1920x1080")]
        public static void Set1920x1080()
        {
            gameView.Close();
            SetGameViewSize(1920, 1080);
        }

        static EditorWindow _gameView = null;

        static EditorWindow gameView
        {
            get
            {
                if (_gameView == null)
                {
                    _gameView = GetMainGameView();
                }
                return _gameView;
            }
        }

        static readonly int toolbarHeight = 17;
        static readonly string layout = "Temp/foriero_gameview_layout.wlt";

        static void SetGameViewSize(int width, int height)
        {
            gameView.position = new Rect(0f, 0f, width, height + toolbarHeight);
            gameView.Repaint();
        }

        static EditorWindow GetMainGameView()
        {
            EditorApplication.ExecuteMenuItem("Window/Game");
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetMainGameView.Invoke(null, null);
            var gameView = (EditorWindow)Res;
            PropertyInfo selectedSizeIndex = T.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.NonPublic);
            selectedSizeIndex.SetValue(gameView, 0, null);
            return gameView;
        }

        static void SaveLayout()
        {
            System.Type T = System.Type.GetType("UnityEditor.WindowLayout,UnityEditor");
            var MI = T.GetMethod("SaveWindowLayout");
            MI.Invoke(null, new object[] { layout });
        }

        static void LoadLayout()
        {
            EditorUtility.LoadWindowLayout(layout);
        }
    }
}