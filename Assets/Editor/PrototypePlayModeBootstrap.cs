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
            SceneAsset prototypeScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
            if (prototypeScene != null && EditorSceneManager.playModeStartScene != prototypeScene)
            {
                EditorSceneManager.playModeStartScene = prototypeScene;
            }
        }
    }
}
