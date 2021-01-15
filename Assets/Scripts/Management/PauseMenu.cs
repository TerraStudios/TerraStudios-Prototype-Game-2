﻿//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement.
// All of the contents of this script are Confidential. Distributing or using them for your own needs is prohibited.
// Destroy the file immediately if you are not one of the parties involved.
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
