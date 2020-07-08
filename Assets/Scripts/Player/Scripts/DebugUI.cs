using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    [Header("Components")]
    public EconomyManager EconomyManager;

    [Header("UI Components")]
    public GameObject SuperSecretPanel;
    public GameObject GraphyGO;
    public GameObject ElectricityStatsPanel;

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.RightControl))
        {
            if (SuperSecretPanel.activeSelf)
                SuperSecretPanel.SetActive(false);
            else
                SuperSecretPanel.SetActive(true);
        }
    }

    public void OnBalanceUpdated(string newValue)
    {
        bool isParsable = decimal.TryParse(newValue, out decimal newBalance);

        if (isParsable)
            EconomyManager.Balance = newBalance;
        else
            Debug.LogError("This field only accepts ints!");
    }

    public void OnUIEnableGraphy(bool state)
    {
        GraphyGO.SetActive(state);
    }

    public void OnEnableElectricityStats(bool state)
    {
        ElectricityStatsPanel.SetActive(state);
    }
}
