using UnityEngine;
using UnityEngine.InputSystem;

namespace Stormframe.Player
{
    [RequireComponent(typeof(Camera))]
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _distance = 9f;
        [SerializeField] private float _yaw = 35f;
        [SerializeField] private float _pitch = 35f;
        [SerializeField] private float _collisionRadius = 0.25f;
        [SerializeField] private float _focusSmoothTime = 0.12f;
        private Camera _camera;
        private CameraMode _mode = CameraMode.Medium;
        private Vector3 _buildingFocus;
        private Vector3 _smoothedFocus;
        private Vector3 _focusVelocity;
        private bool _hasBuildingFocus;

        public CameraMode Mode => _mode;

        public void SetTarget(Transform target)
        {
            _target = target;
            _smoothedFocus = NormalFocus;
        }

        public void SetBuildingFocus(Vector3 worldPosition)
        {
            if (_target == null || Vector3.Distance(_target.position, worldPosition) > 24f) return;
            _buildingFocus = worldPosition + Vector3.up * 0.5f;
            _hasBuildingFocus = true;
        }

        private Vector3 NormalFocus => _target.position + Vector3.up * 1.25f;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            ReadModeSelection();
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
                if (_camera.orthographic)
                {
                    _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - scroll, 3f, 18f);
                }
                else
                {
                    _distance = Mathf.Clamp(_distance - scroll, MinimumDistance, MaximumDistance);
                }
            }

            Vector3 targetFocus = UsesBuildingFocus && _hasBuildingFocus
                ? _buildingFocus
                : NormalFocus;
            _smoothedFocus = Vector3.SmoothDamp(
                _smoothedFocus,
                targetFocus,
                ref _focusVelocity,
                _focusSmoothTime);

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 backward = -(rotation * Vector3.forward);
            Vector3 desired = _smoothedFocus + backward * _distance;

            if (Physics.SphereCast(
                    _smoothedFocus,
                    _collisionRadius,
                    backward,
                    out RaycastHit hit,
                    _distance,
                    ~LayerMask.GetMask("Ignore Raycast"),
                    QueryTriggerInteraction.Ignore))
            {
                desired = _smoothedFocus + backward * Mathf.Max(hit.distance - 0.15f, 0.5f);
            }

            transform.SetPositionAndRotation(desired, rotation);
        }

        private bool UsesBuildingFocus =>
            _mode == CameraMode.BuildingOrbit || _mode == CameraMode.Isometric;

        private float MinimumDistance => _mode switch
        {
            CameraMode.Close => 3.5f,
            CameraMode.High => 8f,
            _ => 5f
        };

        private float MaximumDistance => _mode switch
        {
            CameraMode.Close => 9f,
            CameraMode.High => 24f,
            CameraMode.BuildingOrbit => 24f,
            _ => 18f
        };

        private void ReadModeSelection()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (keyboard.f1Key.wasPressedThisFrame) SetMode(CameraMode.Close);
            if (keyboard.f2Key.wasPressedThisFrame) SetMode(CameraMode.Medium);
            if (keyboard.f3Key.wasPressedThisFrame) SetMode(CameraMode.High);
            if (keyboard.f4Key.wasPressedThisFrame) SetMode(CameraMode.BuildingOrbit);
            if (keyboard.f5Key.wasPressedThisFrame) SetMode(CameraMode.Isometric);
        }

        public void SetMode(CameraMode mode)
        {
            _mode = mode;
            _camera.orthographic = mode == CameraMode.Isometric;
            _camera.fieldOfView = mode switch
            {
                CameraMode.Close => 62f,
                CameraMode.High => 52f,
                CameraMode.BuildingOrbit => 50f,
                _ => 56f
            };

            switch (mode)
            {
                case CameraMode.Close:
                    _distance = 5.5f;
                    _pitch = 24f;
                    break;
                case CameraMode.Medium:
                    _distance = 9f;
                    _pitch = 35f;
                    break;
                case CameraMode.High:
                    _distance = 14f;
                    _pitch = 58f;
                    break;
                case CameraMode.BuildingOrbit:
                    _distance = 11f;
                    _pitch = 42f;
                    break;
                case CameraMode.Isometric:
                    _distance = 15f;
                    _pitch = 45f;
                    _yaw = 45f;
                    _camera.orthographicSize = 8f;
                    break;
            }
        }

        private void OnGUI()
        {
            string label = _mode switch
            {
                CameraMode.Close => "F1 Close third-person",
                CameraMode.Medium => "F2 Medium third-person",
                CameraMode.High => "F3 High third-person",
                CameraMode.BuildingOrbit => "F4 Building orbit",
                CameraMode.Isometric => "F5 Orthographic / isometric",
                _ => _mode.ToString()
            };
            GUI.Box(new Rect(Screen.width - 260, 16, 244, 50), "Camera Laboratory");
            GUI.Label(new Rect(Screen.width - 248, 40, 220, 22), label);
        }
    }
}
