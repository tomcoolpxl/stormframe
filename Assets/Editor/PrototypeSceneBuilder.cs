using Stormframe.Construction;
using Stormframe.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Stormframe.Editor
{
    public static class PrototypeSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Prototype.unity";

        [MenuItem("Tools/Stormframe/Create Prototype Scene")]
        public static void BuildPrototypeScene()
        {
            EnsureFolder("Assets", "Scenes");
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateGround();
            CreateLighting();
            GameObject player = CreatePlayer();
            CreateCamera(player.transform);
            new GameObject("Construction").AddComponent<ConstructionController>();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
            PlayerSettings.companyName = "Stormframe";
            PlayerSettings.productName = "Stormframe Prototype";
            Selection.activeGameObject = player;
            Debug.Log($"Created prototype scene at {ScenePath}");
        }

        private static void CreateGround()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Ground";
            ground.transform.position = new Vector3(0f, -0.25f, 0f);
            ground.transform.localScale = new Vector3(60f, 0.5f, 60f);
            ground.GetComponent<Renderer>().sharedMaterial = CreateMaterial(
                "Ground Material",
                new Color(0.24f, 0.42f, 0.24f));
        }

        private static void CreateLighting()
        {
            var lightObject = new GameObject("Directional Light");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.shadows = LightShadows.Soft;
            lightObject.transform.rotation = Quaternion.Euler(48f, -32f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.55f, 0.66f, 0.78f);
            RenderSettings.ambientGroundColor = new Color(0.16f, 0.18f, 0.14f);
        }

        private static GameObject CreatePlayer()
        {
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Placeholder Player";
            player.layer = LayerMask.NameToLayer("Ignore Raycast");
            player.transform.position = new Vector3(0f, 1f, -5f);
            Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
            var controller = player.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.45f;
            controller.center = Vector3.zero;
            player.AddComponent<ThirdPersonMotor>();
            player.GetComponent<Renderer>().sharedMaterial = CreateMaterial(
                "Player Material",
                new Color(0.92f, 0.68f, 0.18f));
            return player;
        }

        private static void CreateCamera(Transform target)
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            cameraObject.AddComponent<ThirdPersonCamera>().SetTarget(target);
        }

        private static Material CreateMaterial(string name, Color color)
        {
            var material = new Material(Shader.Find("Standard"))
            {
                name = name,
                color = color
            };
            return material;
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, child);
        }
    }
}
