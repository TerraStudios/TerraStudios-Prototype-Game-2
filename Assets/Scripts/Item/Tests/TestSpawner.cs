using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (BoxCollider))]
public class TestSpawner : MonoBehaviour
{
    public int spawnEverySeconds;
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
