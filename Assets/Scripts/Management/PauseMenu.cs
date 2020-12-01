using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMenuPanel;

    [HideInInspector] public static bool isOpen;
    public static bool wasPaused;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenuPanel.activeSelf)
            {
                isOpen = false;
                wasPaused = TimeEngine.IsPaused;
                TimeEngine.IsPaused = false;
                PauseMenuPanel.SetActive(false);
            }

            else
            {
                isOpen = true;
                wasPaused = TimeEngine.IsPaused;
                TimeEngine.IsPaused = true;
                PauseMenuPanel.SetActive(true);
            }
        }
    }
}
