using UnityEngine;

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

            if (Input.GetMouseButton(2))
            {
                _yaw += Input.GetAxis("Mouse X") * 4f;
                _pitch = Mathf.Clamp(_pitch - Input.GetAxis("Mouse Y") * 3f, 15f, 75f);
            }

            _distance = Mathf.Clamp(_distance - Input.mouseScrollDelta.y, 4f, 18f);
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
