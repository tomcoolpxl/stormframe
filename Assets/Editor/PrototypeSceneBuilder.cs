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
            CreateCrashSite();
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
            var player = new GameObject("Stranded Robot");
            player.layer = LayerMask.NameToLayer("Ignore Raycast");
            player.transform.position = new Vector3(0f, 1f, -5f);
            var controller = player.AddComponent<CharacterController>();
            controller.height = 1.65f;
            controller.radius = 0.5f;
            controller.center = new Vector3(0f, -0.1f, 0f);
            player.AddComponent<ThirdPersonMotor>();

            var visualRoot = new GameObject("Robot Visual");
            visualRoot.layer = player.layer;
            visualRoot.transform.SetParent(player.transform, false);
            Material shell = CreateMaterial("Robot Shell", new Color(0.12f, 0.16f, 0.19f));
            Material accent = CreateMaterial("Robot Accent", new Color(0.92f, 0.42f, 0.12f));
            Material glow = CreateEmissiveMaterial("Robot Glow", new Color(0.1f, 0.85f, 1f));

            CreatePart("Body", PrimitiveType.Sphere, visualRoot.transform, new Vector3(0f, -0.05f, 0f), new Vector3(0.82f, 0.62f, 0.68f), shell);
            CreatePart("Head", PrimitiveType.Cube, visualRoot.transform, new Vector3(0f, 0.46f, 0.03f), new Vector3(0.68f, 0.42f, 0.54f), shell);
            CreatePart("Eye", PrimitiveType.Cube, visualRoot.transform, new Vector3(0f, 0.49f, 0.315f), new Vector3(0.38f, 0.09f, 0.035f), glow);
            CreatePart("Left Stabilizer", PrimitiveType.Sphere, visualRoot.transform, new Vector3(-0.5f, -0.18f, 0f), new Vector3(0.25f, 0.32f, 0.3f), accent);
            CreatePart("Right Stabilizer", PrimitiveType.Sphere, visualRoot.transform, new Vector3(0.5f, -0.18f, 0f), new Vector3(0.25f, 0.32f, 0.3f), accent);
            CreatePart("Hover Emitter", PrimitiveType.Cylinder, visualRoot.transform, new Vector3(0f, -0.43f, 0f), new Vector3(0.34f, 0.035f, 0.34f), glow);

            var scanner = new GameObject("Scanner Pivot");
            scanner.layer = player.layer;
            scanner.transform.SetParent(visualRoot.transform, false);
            CreatePart("Scanner Mast", PrimitiveType.Cylinder, scanner.transform, new Vector3(0f, 0.78f, 0f), new Vector3(0.045f, 0.18f, 0.045f), accent);
            CreatePart("Scanner Tip", PrimitiveType.Sphere, scanner.transform, new Vector3(0.16f, 0.98f, 0f), Vector3.one * 0.12f, glow);

            var eyeLightObject = new GameObject("Eye Light");
            eyeLightObject.layer = player.layer;
            eyeLightObject.transform.SetParent(visualRoot.transform, false);
            eyeLightObject.transform.localPosition = new Vector3(0f, 0.5f, 0.38f);
            Light eyeLight = eyeLightObject.AddComponent<Light>();
            eyeLight.type = LightType.Point;
            eyeLight.color = new Color(0.1f, 0.85f, 1f);
            eyeLight.range = 3.5f;
            eyeLight.intensity = 1f;

            player.AddComponent<RobotVisualAnimator>().Configure(visualRoot.transform, scanner.transform, eyeLight);
            return player;
        }

        private static void CreateCrashSite()
        {
            var crashSite = new GameObject("Crash Site");
            crashSite.transform.position = new Vector3(4f, 0f, -1f);
            Material hull = CreateMaterial("Crash Hull", new Color(0.1f, 0.12f, 0.14f));
            Material accent = CreateMaterial("Crash Accent", new Color(0.85f, 0.3f, 0.08f));
            Material glow = CreateEmissiveMaterial("Beacon Glow", new Color(0.1f, 0.85f, 1f));

            CreatePart("Scorched Ground", PrimitiveType.Cylinder, crashSite.transform, new Vector3(0f, 0.02f, 0f), new Vector3(2.7f, 0.015f, 1.8f), CreateMaterial("Scorch", new Color(0.08f, 0.09f, 0.07f)));
            GameObject pod = CreatePart("Broken Pod", PrimitiveType.Capsule, crashSite.transform, new Vector3(0f, 0.48f, 0f), new Vector3(0.62f, 1.1f, 0.62f), hull);
            pod.transform.localRotation = Quaternion.Euler(0f, 0f, 72f);
            GameObject panel = CreatePart("Broken Panel", PrimitiveType.Cube, crashSite.transform, new Vector3(1.15f, 0.18f, 0.45f), new Vector3(0.9f, 0.08f, 0.55f), accent);
            panel.transform.localRotation = Quaternion.Euler(8f, 28f, -12f);
            CreatePart("Beacon Mast", PrimitiveType.Cylinder, crashSite.transform, new Vector3(-0.85f, 0.55f, -0.25f), new Vector3(0.06f, 0.55f, 0.06f), hull);
            CreatePart("Beacon", PrimitiveType.Sphere, crashSite.transform, new Vector3(-0.85f, 1.12f, -0.25f), Vector3.one * 0.18f, glow);

            var beaconLightObject = new GameObject("Beacon Light");
            beaconLightObject.transform.SetParent(crashSite.transform, false);
            beaconLightObject.transform.localPosition = new Vector3(-0.85f, 1.12f, -0.25f);
            Light beaconLight = beaconLightObject.AddComponent<Light>();
            beaconLight.type = LightType.Point;
            beaconLight.color = new Color(0.1f, 0.85f, 1f);
            beaconLight.range = 5f;
            beaconLight.intensity = 1.4f;
        }

        private static GameObject CreatePart(
            string name,
            PrimitiveType primitive,
            Transform parent,
            Vector3 position,
            Vector3 scale,
            Material material)
        {
            GameObject part = GameObject.CreatePrimitive(primitive);
            part.name = name;
            part.layer = LayerMask.NameToLayer("Ignore Raycast");
            part.transform.SetParent(parent, false);
            part.transform.localPosition = position;
            part.transform.localScale = scale;
            Object.DestroyImmediate(part.GetComponent<Collider>());
            part.GetComponent<Renderer>().sharedMaterial = material;
            return part;
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

        private static Material CreateEmissiveMaterial(string name, Color color)
        {
            Material material = CreateMaterial(name, color);
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * 1.8f);
            return material;
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, child);
        }
    }
}
