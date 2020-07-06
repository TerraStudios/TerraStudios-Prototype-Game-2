using System.Collections.Generic;
using UnityEngine;

//TODO: maybe find a more elegant queue system than using a list and looping
public class ObjectPoolManager : MonoBehaviour
{

    private Dictionary<string, Queue<PoolInstance>> pooledObjects = new Dictionary<string, Queue<PoolInstance>>();

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
        string key = prefab.name;

        if (!pooledObjects.ContainsKey(key))
        {
            pooledObjects.Add(key, new Queue<PoolInstance>());
            GameObject parent = new GameObject(prefab.name + " ObjectPool");

            for (int i = 0; i < poolSize; i++)
            {
                PoolInstance newInstance = new PoolInstance(Instantiate(prefab), parent.transform);
                newInstance.Disable();
                pooledObjects[key].Enqueue(newInstance);
            }
        }
    }

    /// <summary>
    /// Attempts to reuse an existing object in the pool, setting a position and rotation
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public GameObject ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string key = prefab.name;

        if (pooledObjects.ContainsKey(key))
        {

            PoolInstance pooledObject = pooledObjects[key].Dequeue();
            //pooledObjects[key].Enqueue(pooledObject);


            if (pooledObjects[key].Count == 0)
            {
                PoolInstance newInstance = new PoolInstance(Instantiate(prefab), pooledObject.gameObject.transform.parent);
                newInstance.Disable(); //disable to not cause lag 
                pooledObjects[key].Enqueue(newInstance);
                pooledObject = newInstance;
            }

            pooledObject.gameObject.SetActive(true);
            Transform t = pooledObject.gameObject.transform;
            t.position = position;
            t.rotation = rotation;
            return pooledObject.gameObject;
        }

        return null; // This in theory should never happen unless the pool doesn't exist, which would mean you're doing something very wrong.
    }

    public void DestroyObject(GameObject gameObject)
    {

        //gameObject.SetActive(false);

        PoolInstance newInstance = new PoolInstance(gameObject, gameObject.transform.parent);

        newInstance.Disable();
        pooledObjects[gameObject.name.Replace("(Clone)", "")].Enqueue(newInstance); //requeue object for use 
    }





    public class PoolInstance
    {

        public GameObject gameObject;
        public Transform parent;


        public PoolInstance(GameObject gameObject, Transform parent = null)
        {

            this.gameObject = gameObject;
            this.parent = parent;




        }

        public void Disable()
        {
            this.gameObject.SetActive(false);

            if (parent != null)
            {
                this.gameObject.transform.SetParent(parent);
            }
        }


    }
}
