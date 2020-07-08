using UnityEngine;

public class PoolObject : MonoBehaviour {

    protected void Destroy()
    {
        gameObject.SetActive(false);
    }

}