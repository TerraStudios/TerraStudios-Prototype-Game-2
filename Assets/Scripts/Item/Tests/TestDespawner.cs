//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement.
// All of the contents of this script are Confidential. Distributing or using them for your own needs is prohibited.
// Destroy the file immediately if you are not one of the parties involved.
//

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
