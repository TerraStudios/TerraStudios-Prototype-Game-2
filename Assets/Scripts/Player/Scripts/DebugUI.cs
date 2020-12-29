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

            for (int i = 0; i < ItemManagement.ItemManagement.db.Count; i++)
            {
                ItemData data = ItemManagement.ItemManagement.db[i];
                testItemDropdown.options.Add(new TMP_Dropdown.OptionData() { text = data.name });
            }

            testItemDropdown.RefreshShownValue();

            testItemDropdown.onValueChanged.AddListener(delegate { OnItemSelected(testItemDropdown); });

            BuildingManager.testItemToSpawn = ItemManagement.ItemManagement.db[0];
        }

        public void OnItemSelected(TMP_Dropdown changed)
        {
            BuildingManager.testItemToSpawn = ItemManagement.ItemManagement.db[changed.value];
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
                foreach (Building registered in BuildingSystem.RegisteredBuildings)
                {
                    registered.mc.buildingIOManager.VisualizeAll();
                }
            }
            else
            {
                // hide all
                GridManager.Instance.forceVisualizeAll = false;
                foreach (Building registered in BuildingSystem.RegisteredBuildings)
                {
                    registered.mc.buildingIOManager.DevisualizeAll();
                }
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
