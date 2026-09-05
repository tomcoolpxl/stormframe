using UnityEngine;

namespace Stormframe.Player
{
    public sealed class RobotVisualAnimator : MonoBehaviour
    {
        private Transform _visualRoot;
        private Transform _scanner;
        private Light _eyeLight;
        private Vector3 _restPosition;

        public void Configure(Transform visualRoot, Transform scanner, Light eyeLight)
        {
            _visualRoot = visualRoot;
            _scanner = scanner;
            _eyeLight = eyeLight;
            _restPosition = visualRoot.localPosition;
        }

        private void Update()
        {
            if (_visualRoot == null) return;
            float time = Time.time;
            _visualRoot.localPosition = _restPosition + Vector3.up * (Mathf.Sin(time * 2.4f) * 0.035f);
            if (_scanner != null)
            {
                _scanner.localRotation = Quaternion.Euler(0f, time * 70f, 0f);
            }

            if (_eyeLight != null)
            {
                _eyeLight.intensity = 0.85f + Mathf.Sin(time * 3.1f) * 0.15f;
            }
        }
    }
}
