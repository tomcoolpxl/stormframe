using UnityEngine;
using UnityEngine.InputSystem;

namespace Stormframe.Player
{
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _distance = 9f;
        [SerializeField] private float _yaw = 35f;
        [SerializeField] private float _pitch = 35f;

        public void SetTarget(Transform target)
        {
            _target = target;
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
                _distance = Mathf.Clamp(_distance - mouse.scroll.ReadValue().y / 120f, 4f, 18f);
            }
            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 focus = _target.position + Vector3.up * 1.25f;
            Vector3 desired = focus - rotation * Vector3.forward * _distance;

            if (Physics.Linecast(focus, desired, out RaycastHit hit, ~LayerMask.GetMask("Ignore Raycast")))
            {
                desired = hit.point + hit.normal * 0.2f;
            }

            transform.SetPositionAndRotation(desired, rotation);
        }
    }
}
