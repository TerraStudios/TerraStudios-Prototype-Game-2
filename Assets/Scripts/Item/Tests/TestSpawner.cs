//
// Developed by TerraStudios (https://github.com/TerraStudios)
//
// Copyright(c) 2020-2021 Konstantin Milev (konstantin890 | milev109@gmail.com)
// Copyright(c) 2020-2021 Yerti (UZ9)
//
// The following script has been written by either konstantin890 or Yerti (UZ9) or both.
// This file is covered by the GNU GPL v3 license. Read LICENSE.md for more information.
// Past NDA/MNDA and Confidential notices are revoked and invalid since no agreement took place. Read README.md for more information.
//

using System.Collections;
using ItemManagement;
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
