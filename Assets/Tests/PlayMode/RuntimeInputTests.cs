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
    }
}
