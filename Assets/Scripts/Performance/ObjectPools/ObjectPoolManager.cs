using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{

    private Dictionary<int, Queue<PoolInstance>> pooledObjects = new Dictionary<int, Queue<PoolInstance>>();

    private static ObjectPoolManager _instance;

    public static ObjectPoolManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ObjectPoolManager>();
            }

            return _instance;
        }

    }


    public void CreatePool(GameObject prefab, int poolSize)
    {
        int key = prefab.GetInstanceID();

        if (!pooledObjects.ContainsKey(key))
        {
            pooledObjects.Add(key, new Queue<PoolInstance>());
            GameObject parent = new GameObject(prefab.name + " ObjectPool");

            for (int i = 0; i < poolSize; i++)
            {
                pooledObjects[key].Enqueue(new PoolInstance(prefab, parent.transform));
            }
        }
    }

    /// <summary>
    /// Attempts to reuse an existing object in the pool, setting a position and rotation
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int key = prefab.GetInstanceID();

        if (pooledObjects.ContainsKey(key))
        {

            PoolInstance pooledObject = pooledObjects[key].Dequeue();
            pooledObjects[key].Enqueue(pooledObject);
            

            if (pooledObject.gameObject.activeSelf)
            {
                PoolInstance newInstance = new PoolInstance(prefab, pooledObject.gameObject.transform.parent);
                pooledObjects[key].Enqueue(newInstance);
                pooledObject = newInstance;
            } 

            pooledObject.gameObject.SetActive(true);
            Transform t = pooledObject.gameObject.transform;
            t.position = position;
            t.rotation = rotation;
        }
    }




    public class PoolInstance
    {

        public GameObject gameObject;

        public PoolInstance(GameObject gameObject, Transform parent = null)
        {

            this.gameObject = Instantiate(gameObject) as GameObject;


            this.gameObject.SetActive(false);

            if (parent != null)
            {
                this.gameObject.transform.SetParent(parent);
            }
        }


    }
}
