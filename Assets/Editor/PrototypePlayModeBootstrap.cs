using System;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Stormframe.Editor
{
    [InitializeOnLoad]
    public static class PrototypePlayModeBootstrap
    {
        private const string ScenePath = "Assets/Scenes/Prototype.unity";

        static PrototypePlayModeBootstrap()
        {
            EditorApplication.delayCall += ConfigureStartScene;
        }

        private static void ConfigureStartScene()
        {
            bool runningTests = Array.Exists(
                Environment.GetCommandLineArgs(),
                argument => string.Equals(argument, "-runTests", StringComparison.OrdinalIgnoreCase));
            if (runningTests)
            {
                EditorSceneManager.playModeStartScene = null;
                return;
            }

            SceneAsset prototypeScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
            if (prototypeScene != null && EditorSceneManager.playModeStartScene != prototypeScene)
            {
                EditorSceneManager.playModeStartScene = prototypeScene;
            }
        }
    }
}
