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
            yield return null;
            Release(keyboard.wKey);

            Assert.That(playerObject.transform.position.z, Is.GreaterThan(initialPosition.z));

            Object.Destroy(cameraObject);
            Object.Destroy(playerObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ThirdPersonMotor_WhenSpaceIsPressedWhileGrounded_Jumps()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.transform.position = new Vector3(0f, -0.5f, 0f);
            ground.transform.localScale = new Vector3(10f, 1f, 10f);
            var playerObject = new GameObject("Test Player");
            playerObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            CharacterController controller = playerObject.AddComponent<CharacterController>();
            controller.height = 1.35f;
            controller.radius = 0.38f;
            controller.center = Vector3.zero;
            playerObject.transform.position = new Vector3(0f, 0.72f, 0f);
            playerObject.AddComponent<ThirdPersonMotor>();
            var cameraObject = new GameObject("Test Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.AddComponent<Camera>();
            cameraObject.transform.rotation = Quaternion.identity;
            Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
            Physics.SyncTransforms();

            yield return null;
            float groundedHeight = playerObject.transform.position.y;
            Press(keyboard.spaceKey);
            yield return null;
            yield return null;
            Release(keyboard.spaceKey);

            Assert.That(playerObject.transform.position.y, Is.GreaterThan(groundedHeight));

            Object.Destroy(cameraObject);
            Object.Destroy(playerObject);
            Object.Destroy(ground);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ThirdPersonCamera_ResetView_UsesSinglePerspectiveSetup()
        {
            var targetObject = new GameObject("Test Target");
            var cameraObject = new GameObject("Test Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            ThirdPersonCamera cameraController = cameraObject.AddComponent<ThirdPersonCamera>();
            cameraController.SetTarget(targetObject.transform);
            camera.orthographic = true;

            cameraController.ResetView();

            Assert.That(camera.orthographic, Is.False);
            Assert.That(camera.fieldOfView, Is.EqualTo(56f));
            Assert.That(
                cameraController.IntendedFocus,
                Is.EqualTo(targetObject.transform.position + Vector3.up * 1.25f));

            Object.Destroy(cameraObject);
            Object.Destroy(targetObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ThirdPersonCamera_F1RefocusesPerspectiveCamera()
        {
            var targetObject = new GameObject("Test Target");
            var cameraObject = new GameObject("Test Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            ThirdPersonCamera cameraController = cameraObject.AddComponent<ThirdPersonCamera>();
            cameraController.SetTarget(targetObject.transform);
            camera.orthographic = true;
            Keyboard keyboard = InputSystem.AddDevice<Keyboard>();

            Press(keyboard.f1Key);
            yield return null;
            Release(keyboard.f1Key);

            Assert.That(camera.orthographic, Is.False);
            Assert.That(camera.fieldOfView, Is.EqualTo(56f));

            Object.Destroy(cameraObject);
            Object.Destroy(targetObject);
            yield return null;
        }
    }
}
