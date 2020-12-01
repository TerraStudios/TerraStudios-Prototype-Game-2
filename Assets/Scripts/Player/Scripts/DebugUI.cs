using TMPro;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    [Header("Components")]
    public EconomyManager EconomyManager;

    [Header("UI Components")]
    public GameObject SuperSecretPanel;
    public GameObject GraphyGO;
    public GameObject ElectricityStatsPanel;
    private bool isItemsDropdownLoaded;
    public TMP_Dropdown testItemDropdown;

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.RightControl))
        {
            if (SuperSecretPanel.activeSelf)
                SuperSecretPanel.SetActive(false);
            else
            {
                if (!isItemsDropdownLoaded)
                    LoadItemsDropdown();
                SuperSecretPanel.SetActive(true);
            }

        }
    }

    private void LoadItemsDropdown()
    {
        isItemsDropdownLoaded = true;

        for (int i = 0; i < ItemManagement.db.Count; i++)
        {
            ItemData data = ItemManagement.db[i];
            testItemDropdown.options.Add(new TMP_Dropdown.OptionData() { text = data.name });
        }

        testItemDropdown.RefreshShownValue();

        testItemDropdown.onValueChanged.AddListener(delegate { OnItemSelected(testItemDropdown); });

        BuildingManager.testItemToSpawn = ItemManagement.db[0];
    }

    public void OnItemSelected(TMP_Dropdown changed)
    {
        BuildingManager.testItemToSpawn = ItemManagement.db[changed.value];
    }

    public void OnBalanceUpdated(string newValue)
    {
        bool isParsable = decimal.TryParse(newValue, out decimal newBalance);

        if (isParsable)
            EconomyManager.Balance = newBalance;
        else
            Debug.LogError("This field only accepts ints!");
    }

    public void OnDisplayAllArrows(bool state)
    {
        if (state)
        {
            // show all
            GridManager.instance.forceVisualizeAll = true;
            foreach (Building registered in BuildingSystem.RegisteredBuildings)
            {
                registered.mc.BuildingIOManager.VisualizeAll();
            }
        }
        else
        {
            // hide all
            GridManager.instance.forceVisualizeAll = false;
            foreach (Building registered in BuildingSystem.RegisteredBuildings)
            {
                registered.mc.BuildingIOManager.DevisualizeAll();
            }
        }
    }

    public void OnUIEnableGraphy(bool state)
    {
        GraphyGO.SetActive(state);
    }

    public void OnEnableElectricityStats(bool state)
    {
        ElectricityStatsPanel.SetActive(state);
    }

    public void OnShowIOAttachCollisions(bool state)
    {
        GridManager.instance.debugMode = state;
    }
}
