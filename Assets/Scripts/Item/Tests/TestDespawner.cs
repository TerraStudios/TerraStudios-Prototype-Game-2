using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TestDespawner : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(11))
            ObjectPoolManager.instance.DestroyObject(other.gameObject);
        //Destroy(other.gameObject);
    }
}
