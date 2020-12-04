using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        AsyncOperation load2 = SceneManager.LoadSceneAsync(staticSceneIndex, LoadSceneMode.Additive);

        while (!load2.isDone)
            yield return null;
    }
}