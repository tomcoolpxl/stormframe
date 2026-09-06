using UnityEngine;
using UnityEngine.InputSystem;

namespace Stormframe.Player
{
    [RequireComponent(typeof(Camera))]
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _distance = DefaultDistance;
        [SerializeField] private float _yaw = DefaultYaw;
        [SerializeField] private float _pitch = DefaultPitch;
        [SerializeField] private float _collisionRadius = 0.25f;
        [SerializeField] private float _focusSmoothTime = 0.12f;
        [SerializeField] private float _collisionSmoothTime = 0.08f;
        private Camera _camera;
        private Vector3 _smoothedFocus;
        private Vector3 _focusVelocity;
        private float _collisionDistance;
        private float _collisionVelocity;

        private const float DefaultDistance = 9f;
        private const float DefaultYaw = 35f;
        private const float DefaultPitch = 35f;
        private const float MinimumDistance = 5f;
        private const float MaximumDistance = 18f;
        private const float MinimumCollisionDistance = 3.5f;

        public Vector3 IntendedFocus => NormalFocus;

        private Vector3 NormalFocus => _target.position + Vector3.up * 1.25f;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        public void SetTarget(Transform target)
        {
            _target = target;
            ResetView();
        }

        public void ResetView()
        {
            _distance = DefaultDistance;
            _yaw = DefaultYaw;
            _pitch = DefaultPitch;
            _collisionDistance = _distance;
            _collisionVelocity = 0f;
            _focusVelocity = Vector3.zero;
            if (_target != null) _smoothedFocus = NormalFocus;
            if (_camera == null) _camera = GetComponent<Camera>();
            _camera.orthographic = false;
            _camera.fieldOfView = 56f;
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.f1Key.wasPressedThisFrame) ResetView();
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            Mouse mouse = Mouse.current;
            if (mouse != null && mouse.middleButton.isPressed)
            {
                Vector2 delta = mouse.delta.ReadValue();
                _yaw += delta.x * 0.2f;
                _pitch = Mathf.Clamp(_pitch - delta.y * 0.15f, 15f, 75f);
            }

            if (mouse != null)
            {
                float scroll = mouse.scroll.ReadValue().y / 120f;
                _distance = Mathf.Clamp(_distance - scroll, MinimumDistance, MaximumDistance);
            }

            _smoothedFocus = Vector3.SmoothDamp(
                _smoothedFocus,
                NormalFocus,
                ref _focusVelocity,
                _focusSmoothTime);

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 backward = -(rotation * Vector3.forward);
            float desiredCollisionDistance = _distance;
            if (Physics.SphereCast(
                    _smoothedFocus,
                    _collisionRadius,
                    backward,
                    out RaycastHit hit,
                    _distance,
                    ~LayerMask.GetMask("Ignore Raycast"),
                    QueryTriggerInteraction.Ignore))
            {
                desiredCollisionDistance = Mathf.Max(
                    hit.distance - 0.15f,
                    MinimumCollisionDistance);
            }

            if (_collisionDistance <= 0f) _collisionDistance = _distance;
            _collisionDistance = Mathf.SmoothDamp(
                _collisionDistance,
                desiredCollisionDistance,
                ref _collisionVelocity,
                _collisionSmoothTime);
            Vector3 desired = _smoothedFocus + backward * _collisionDistance;
            transform.SetPositionAndRotation(desired, rotation);
        }
    }
}
