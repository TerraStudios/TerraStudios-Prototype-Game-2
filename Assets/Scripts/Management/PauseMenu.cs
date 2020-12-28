using TimeSystem;
using UnityEngine;
using UnityEngine.InputSystem;

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

        public void ShowTrigger(InputAction.CallbackContext context)
        {
            if (context.performed)
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
