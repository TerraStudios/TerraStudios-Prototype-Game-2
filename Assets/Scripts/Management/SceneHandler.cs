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
