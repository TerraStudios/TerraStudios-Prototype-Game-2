using ItemManagement;
using System.Collections;
using UnityEngine;
using Utilities;

namespace DebugTools
{
    public class TestSpawner : MonoBehaviour
    {
        public float spawnEverySeconds;
        public ItemData itemToSpawn;

        // Start is called before the first frame update
        private void Start()
        {
            StartCoroutine(StartLoop());
        }

        private IEnumerator StartLoop()
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(LoopSpawnItem());
        }

        private IEnumerator LoopSpawnItem()
        {
            yield return new WaitForSeconds(spawnEverySeconds);

            ObjectPoolManager.Instance.ReuseObject(itemToSpawn.obj.gameObject, transform.position, Quaternion.identity);

            //Instantiate(itemToSpawn.obj, transform.position, Quaternion.identity);
            StartCoroutine(LoopSpawnItem());
        }
    }
}
