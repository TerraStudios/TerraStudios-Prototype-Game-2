using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for instantiating and destroying GameObjects efficiently by reusing them.
/// </summary>
//TODO: maybe find a more elegant queue system than using a list and looping
public class ObjectPoolManager : MonoBehaviour
{
    public readonly int defaultPoolSize = 10;

    private Dictionary<string, Queue<PoolInstance>> pooledObjects = new Dictionary<string, Queue<PoolInstance>>();

    /// <summary>
    /// Returns the instance of the <see cref="ObjectPoolManager"/>.
    /// </summary>
    public static ObjectPoolManager Instance;

    private void Awake() => Instance = this;

    /// <summary>
    /// Initializes an object pool for a prefab with an initial size. If the amount of active objects exceeds the initial pool size more objects will be instantiated to compensate.
    /// </summary>
    /// <example>
    /// Below is an example of how to use the <see cref="CreatePool(GameObject, int)"/> method.
    /// <code>
    /// ObjectPoolManager.Instance.CreatePool(prefab, 100); //Creates an initial size of 100
    /// </code>
    /// </example>
    /// <param name="prefab">The <see cref="GameObject"/> the pool will contain</param>
    /// <param name="poolSize">The initial size for the pool</param>
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
    /// <example>
    /// Below is an example of how to instantiate a game object from the pool created using <see cref="ObjectPoolManager.CreatePool(GameObject, int)"/>. 
    /// <code>
    /// GameObject instantiatedObject = ObjectPoolManager.Instance.ReuseObject(prefab, new Vector3(0, 30, 0), Quaternion.identity);
    /// //do whatever with the game object
    /// </code>
    /// 
    /// </example>
    /// <param name="prefab">The prefab to instantiate</param>
    /// <param name="position">The position for the prefab to spawn as</param>
    /// <param name="rotation">The rotation for the prefab to be angled at</param>
    public GameObject ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string key = prefab.name;

        if (!pooledObjects.ContainsKey(key))
        {
            //If a pool does not already exist for the GameObject, create a new one with the default pool size
            CreatePool(prefab, defaultPoolSize);
        }

        PoolInstance pooledObject = pooledObjects[key].Dequeue();
        //pooledObjects[key].Enqueue(pooledObject);)

        if (pooledObjects[key].Count == 0)
        {
            PoolInstance newInstance = new PoolInstance(Instantiate(prefab), pooledObject.gameObject.transform.parent);
            newInstance.Disable(); //disable to not cause lag
            pooledObjects[key].Enqueue(newInstance);
        }

        pooledObject.gameObject.SetActive(true);
        Transform t = pooledObject.gameObject.transform;
        t.position = position;
        t.rotation = rotation;
        return pooledObject.gameObject;
    }

    /// <summary>
    /// Attempts to destroy a <see cref="GameObject"/> that was previously instantiated using <see cref="ReuseObject(GameObject, Vector3, Quaternion)"/>
    /// </summary>
    /// <example>
    /// Below is an example of how to destroy a <see cref="GameObject"/>, freeing up space in the <see cref="ObjectPoolManager"/>
    /// <code>
    /// ObjectPoolManager.Instance.DestroyObject(instantiatedObject);
    /// </code>
    /// </example>
    /// <param name="gameObject">The <see cref="GameObject"/> to be destroyed</param>
    public void DestroyObject(GameObject gameObject)
    {
        PoolInstance newInstance = new PoolInstance(gameObject, gameObject.transform.parent);

        newInstance.Disable();
        pooledObjects[gameObject.name.Replace("(Clone)", "")].Enqueue(newInstance); //requeue object for use
    }

    /// <summary>
    /// Private class used for storing information relating to the <see cref="ObjectPoolManager"/> data.
    /// </summary>
    private class PoolInstance
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
            gameObject.SetActive(false);

            if (parent != null)
            {
                gameObject.transform.SetParent(parent);
            }
        }
    }
}
