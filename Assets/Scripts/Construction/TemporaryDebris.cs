using UnityEngine;

namespace Stormframe.Construction
{
    public sealed class TemporaryDebris : MonoBehaviour
    {
        public void Initialize(float lifetimeSeconds = 4f)
        {
            Destroy(gameObject, lifetimeSeconds);
        }
    }
}
