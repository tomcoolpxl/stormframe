using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Stormframe.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class ThirdPersonMotor : MonoBehaviour
    {
        [SerializeField] private float _speed = 6f;
        [SerializeField] private float _turnSpeed = 14f;
        [SerializeField] private float _gravity = -20f;
        private CharacterController _controller;
        private float _verticalSpeed;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;

            Transform cameraTransform = Camera.main.transform;
            Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
            float horizontal = ReadAxis(keyboard.aKey, keyboard.dKey)
                + ReadAxis(keyboard.leftArrowKey, keyboard.rightArrowKey);
            float vertical = ReadAxis(keyboard.sKey, keyboard.wKey)
                + ReadAxis(keyboard.downArrowKey, keyboard.upArrowKey);
            Vector3 input = right * horizontal + forward * vertical;
            input = Vector3.ClampMagnitude(input, 1f);

            if (input.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(input),
                    _turnSpeed * Time.deltaTime);
            }

            if (_controller.isGrounded && _verticalSpeed < 0f) _verticalSpeed = -2f;
            _verticalSpeed += _gravity * Time.deltaTime;
            _controller.Move((input * _speed + Vector3.up * _verticalSpeed) * Time.deltaTime);
        }

        private static float ReadAxis(KeyControl negative, KeyControl positive)
        {
            return (positive.isPressed ? 1f : 0f) - (negative.isPressed ? 1f : 0f);
        }
    }
}
