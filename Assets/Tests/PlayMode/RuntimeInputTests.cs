using System.Collections;
using NUnit.Framework;
using Stormframe.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace Stormframe.Tests
{
    public sealed class RuntimeInputTests : InputTestFixture
    {
        [UnityTest]
        public IEnumerator ThirdPersonMotor_WhenWIsPressed_MovesPlayerForward()
        {
            var cameraObject = new GameObject("Test Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.AddComponent<Camera>();
            cameraObject.transform.rotation = Quaternion.identity;

            var playerObject = new GameObject("Test Player");
            playerObject.AddComponent<CharacterController>();
            playerObject.AddComponent<ThirdPersonMotor>();
            Vector3 initialPosition = playerObject.transform.position;

            Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
            Press(keyboard.wKey);
            yield return null;
            Release(keyboard.wKey);

            Assert.That(playerObject.transform.position.z, Is.GreaterThan(initialPosition.z));

            Object.Destroy(cameraObject);
            Object.Destroy(playerObject);
        }

        [UnityTest]
        public IEnumerator ThirdPersonCamera_WhenIsometricSelected_UsesOrthographicLens()
        {
            var targetObject = new GameObject("Test Target");
            var cameraObject = new GameObject("Test Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            ThirdPersonCamera cameraController = cameraObject.AddComponent<ThirdPersonCamera>();
            cameraController.SetTarget(targetObject.transform);

            cameraController.SetMode(CameraMode.Isometric);

            Assert.That(cameraController.Mode, Is.EqualTo(CameraMode.Isometric));
            Assert.That(camera.orthographic, Is.True);

            Object.Destroy(cameraObject);
            Object.Destroy(targetObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ThirdPersonCamera_BuildingFocus_RemainsNearPlayer()
        {
            var targetObject = new GameObject("Test Target");
            var cameraObject = new GameObject("Test Camera");
            cameraObject.AddComponent<Camera>();
            ThirdPersonCamera cameraController = cameraObject.AddComponent<ThirdPersonCamera>();
            cameraController.SetTarget(targetObject.transform);
            cameraController.SetMode(CameraMode.BuildingOrbit);

            cameraController.SetBuildingFocus(new Vector3(20f, 0f, 0f));

            Vector3 playerFocus = targetObject.transform.position + Vector3.up * 1.25f;
            Assert.That(Vector3.Distance(cameraController.IntendedFocus, playerFocus), Is.LessThanOrEqualTo(4.001f));

            Object.Destroy(cameraObject);
            Object.Destroy(targetObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ThirdPersonCamera_MediumMode_ClearsBuildingFocus()
        {
            var targetObject = new GameObject("Test Target");
            var cameraObject = new GameObject("Test Camera");
            cameraObject.AddComponent<Camera>();
            ThirdPersonCamera cameraController = cameraObject.AddComponent<ThirdPersonCamera>();
            cameraController.SetTarget(targetObject.transform);
            cameraController.SetMode(CameraMode.BuildingOrbit);
            cameraController.SetBuildingFocus(new Vector3(8f, 0f, 0f));

            cameraController.SetMode(CameraMode.Medium);

            Assert.That(
                cameraController.IntendedFocus,
                Is.EqualTo(targetObject.transform.position + Vector3.up * 1.25f));

            Object.Destroy(cameraObject);
            Object.Destroy(targetObject);
            yield return null;
        }
    }
}
