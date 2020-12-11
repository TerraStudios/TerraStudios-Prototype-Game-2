using UnityEngine;
using Utilities;

namespace DebugTools
{
    [RequireComponent(typeof(BoxCollider))]
    public class TestDespawner : MonoBehaviour
    {
        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer.Equals(11))
                ObjectPoolManager.Instance.DestroyObject(other.gameObject);
            //Destroy(other.gameObject);
        }
    }
}
