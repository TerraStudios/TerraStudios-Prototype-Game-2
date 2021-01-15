//
// Developped by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement.
// All of the contents of this script are Confidential. Distributing or using them for your own needs is prohibited.
// Destroy the file immediately if you are not one of the parties involved.
//

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoreManagement
{
    /// <summary>
    /// Loads and unloads the game scenes.
    /// Used for loading/unloading game saves.
    /// </summary>
    public class SceneHandler : MonoBehaviour
    {
        public static SceneHandler instance;

        [Header("Build Indexes")]
        public int mainMenuSceneIndex;

        public int baseSceneIndex;
        public int staticSceneIndex;

        private void Awake()
        {
            if (instance)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        public void UnloadMainMenu()
        {
            SceneManager.UnloadSceneAsync(mainMenuSceneIndex);
        }

        public void ReloadGame()
        {
            StartCoroutine(ReloadLevelAction());
        }

        public IEnumerator ReloadLevelAction()
        {
            AsyncOperation load1 = SceneManager.LoadSceneAsync(baseSceneIndex);

            while (!load1.isDone)
                yield return null;

            if (SceneManager.GetSceneByBuildIndex(baseSceneIndex).name != "Prototype Scene 2")
            {
                AsyncOperation load2 = SceneManager.LoadSceneAsync(staticSceneIndex, LoadSceneMode.Additive);

                while (!load2.isDone)
                    yield return null;
            }
        }
    }
}
