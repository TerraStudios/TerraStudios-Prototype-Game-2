﻿//
// Developed by TerraStudios (https://github.com/TerraStudios)
//
// Copyright(c) 2020-2021 Konstantin Milev (konstantin890 | milev109@gmail.com)
// Copyright(c) 2020-2021 Yerti (UZ9)
//
// The following script has been written by either konstantin890 or Yerti (UZ9) or both.
// This file is covered by the GNU GPL v3 license. Read LICENSE.md for more information.
// Past NDA/MNDA and Confidential notices are revoked and invalid since no agreement took place. Read README.md for more information.
//

using System.Collections.Generic;
using BuildingManagement;
using BuildingModules;
using EconomyManagement;
using ItemManagement;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// This system is used for modifying various game properties in runtime.
    /// It can also be used for triggering in-game events.
    /// </summary>
    public class DebugUI : MonoBehaviour
    {
        [Header("Components")]
        public EconomyManager economyManager;

        [Header("UI Components")]
        public GameObject superSecretPanel;
        public GameObject graphyGO;
        public GameObject electricityStatsPanel;
        private bool isItemsDropdownLoaded;
        public TMP_Dropdown testItemDropdown;

        public void ShowTrigger(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (superSecretPanel.activeSelf)
                    superSecretPanel.SetActive(false);
                else
                {
                    if (!isItemsDropdownLoaded)
                        LoadItemsDropdown();
                    superSecretPanel.SetActive(true);
                }
            }
        }

        private void LoadItemsDropdown()
        {
            isItemsDropdownLoaded = true;

            for (int i = 0; i < ItemManagement.ItemManagement.Db.Count; i++)
            {
                ItemData data = ItemManagement.ItemManagement.Db[i];
                testItemDropdown.options.Add(new TMP_Dropdown.OptionData() { text = data.name });
            }

            testItemDropdown.RefreshShownValue();

            testItemDropdown.onValueChanged.AddListener(delegate { OnItemSelected(testItemDropdown); });

            BuildingManager.TestItemToSpawn = ItemManagement.ItemManagement.Db[0];
        }

        public void OnItemSelected(TMP_Dropdown changed)
        {
            BuildingManager.TestItemToSpawn = ItemManagement.ItemManagement.Db[changed.value];
        }

        public void OnBalanceUpdated(string newValue)
        {
            bool isParsable = decimal.TryParse(newValue, out decimal newBalance);

            if (isParsable)
                economyManager.Balance = newBalance;
            else
                Debug.LogError("This field only accepts ints!");
        }

        public void OnDisplayAllArrows(bool state)
        {
            if (state)
            {
                // show all
                GridManager.Instance.forceVisualizeAll = true;
                foreach (List<KeyValuePair<Building, GameObject>> kvp in BuildingSystem.PlacedBuildings.Values)
                    foreach (KeyValuePair<Building, GameObject> buildingKVP in kvp)
                        buildingKVP.Key.mc.buildingIOManager.UpdateArrows();
            }
            else
            {
                // hide all
                GridManager.Instance.forceVisualizeAll = false;
                foreach (List<KeyValuePair<Building, GameObject>> kvp in BuildingSystem.PlacedBuildings.Values)
                    foreach (KeyValuePair<Building, GameObject> buildingKVP in kvp)
                        buildingKVP.Key.mc.buildingIOManager.DestroyArrows();
            }
        }

        public void OnUIEnableGraphy(bool state)
        {
            graphyGO.SetActive(state);
        }

        public void OnEnableElectricityStats(bool state)
        {
            electricityStatsPanel.SetActive(state);
        }

        public void OnShowIOAttachCollisions(bool state)
        {
            GridManager.Instance.debugMode = state;
        }
    }
}
