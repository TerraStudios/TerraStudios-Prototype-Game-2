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
        StartCoroutine(LoopSpawnItem());
    }

    IEnumerator LoopSpawnItem() 
    {
        yield return new WaitForSeconds(spawnEverySeconds);
        Instantiate(itemToSpawn.obj, transform.position, Quaternion.identity);
        StartCoroutine(LoopSpawnItem());
    }
}
