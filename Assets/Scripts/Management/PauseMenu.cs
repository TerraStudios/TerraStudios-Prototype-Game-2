using UnityEngine;
using TimeSystem;

namespace CoreManagement
{
    /// <summary>
    /// Handles the Pause Menu UI.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        public GameObject pauseMenuPanel;

        [HideInInspector] public static bool isOpen;
        public static bool wasPaused;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (pauseMenuPanel.activeSelf)
                {
                    isOpen = false;
                    wasPaused = TimeEngine.IsPaused;
                    TimeEngine.IsPaused = false;
                    pauseMenuPanel.SetActive(false);
                }
                else
                {
                    isOpen = true;
                    wasPaused = TimeEngine.IsPaused;
                    TimeEngine.IsPaused = true;
                    pauseMenuPanel.SetActive(true);
                }
            }
        }
    }
}
