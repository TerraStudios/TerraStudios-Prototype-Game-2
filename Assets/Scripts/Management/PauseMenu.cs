using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMenuPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenuPanel.activeSelf)
            {
                TimeEngine.IsPaused = false;
                PauseMenuPanel.SetActive(false);
            }
                
            else
            {
                TimeEngine.IsPaused = true;
                PauseMenuPanel.SetActive(true);
            } 
        }
    }
}
