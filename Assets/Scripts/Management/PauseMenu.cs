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
        private bool wasCursorShown;

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
                    InputSystemManager.Instance.SwitchToGameplay();
                    Cursor.visible = wasCursorShown;
                    if (wasCursorShown)
                        Cursor.lockState = CursorLockMode.None;
                    else
                        Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    isOpen = true;
                    wasPaused = TimeEngine.IsPaused;
                    TimeEngine.IsPaused = true;
                    pauseMenuPanel.SetActive(true);
                    InputSystemManager.Instance.SwitchToPauseMenu();
                    wasCursorShown = Cursor.visible;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
    }
}
