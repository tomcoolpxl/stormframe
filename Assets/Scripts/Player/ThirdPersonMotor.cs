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
        [SerializeField] private float _jumpHeight = 1.4f;
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

            if (IsGrounded())
            {
                if (_verticalSpeed < 0f) _verticalSpeed = -2f;
                if (keyboard.spaceKey.wasPressedThisFrame)
                {
                    _verticalSpeed = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                }
            }
            _verticalSpeed += _gravity * Time.deltaTime;
            _controller.Move((input * _speed + Vector3.up * _verticalSpeed) * Time.deltaTime);
        }

        private bool IsGrounded()
        {
            if (_controller.isGrounded) return true;
            float probeDistance = _controller.height * 0.5f + 0.12f;
            return Physics.Raycast(
                transform.position,
                Vector3.down,
                probeDistance,
                ~LayerMask.GetMask("Ignore Raycast"),
                QueryTriggerInteraction.Ignore);
        }

        private static float ReadAxis(KeyControl negative, KeyControl positive)
        {
            return (positive.isPressed ? 1f : 0f) - (negative.isPressed ? 1f : 0f);
        }
    }
}
