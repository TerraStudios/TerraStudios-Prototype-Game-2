using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    public float spawnEverySeconds;
    public ItemData itemToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartLoop());
    }

    IEnumerator StartLoop()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(LoopSpawnItem());
    }

    IEnumerator LoopSpawnItem() 
    {
        yield return new WaitForSeconds(spawnEverySeconds);

        ObjectPoolManager.instance.ReuseObject(itemToSpawn.obj.gameObject, transform.position, Quaternion.identity);

        //Instantiate(itemToSpawn.obj, transform.position, Quaternion.identity);
        StartCoroutine(LoopSpawnItem());
    }
}
