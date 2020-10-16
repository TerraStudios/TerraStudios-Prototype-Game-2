using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BuildingManager : BuildingSystem
{
    public static ItemData testItemToSpawn;
    public bool enableDebugSpawn;
    public void EnableDebugSpawn(bool value) => enableDebugSpawn = value;
    [Header("BuildingInfo UI Components")]
    public GameObject BuildingInfo;
    public TMP_Text buildHealth;
    public TMP_Text itemInsideName;

    [Header("IO and Recipe Setup UI Components")]
    public TMP_Dropdown recipeSelection;
    public GameObject IOSelectionPanel;
    public Transform OutputsParent;
    public Transform OutputsSelector;
    private List<Transform> outputSelectorFields = new List<Transform>();

    [Header("Building Indicators")]
    public Transform DirectionIndicator;
    public Transform ArrowIndicator;
    public Transform BrokenIndicator;
    public Transform ErrorIndicator;
    public Transform FixingIndicator;

    [Header("Color Indicators")]
    public Material blueArrow;
    public Material greenArrow;
    public Material redArrow;

    public static BuildingManager instance;

    private void Awake() => instance = this;

    //! Probably has to be moved to BuildingSystem since this script should only handle UI
    public void Start()
    {
        //Create pools for each indicator
        ObjectPoolManager.instance.CreatePool(DirectionIndicator.gameObject, 80);
        ObjectPoolManager.instance.CreatePool(ArrowIndicator.gameObject, 8);
        ObjectPoolManager.instance.CreatePool(BrokenIndicator.gameObject, 50);
        ObjectPoolManager.instance.CreatePool(ErrorIndicator.gameObject, 50);
        ObjectPoolManager.instance.CreatePool(FixingIndicator.gameObject, 50);

        LoadAllBuildingsFromSave();
    }

    //Static because the building manager doesn't have access to BuildingManager, and it doesn't make sense to put it in BuildingIOManager (multiple instances)

    /// <summary>
    /// Event called when the building is selected, sets the UI to active and calls the OnBuildingSelected event
    /// </summary>
    public override void OnBuildingSelected(Building b)
    {
        base.OnBuildingSelected(b);

        b.mc.BuildingIOManager.VisualizeAll();

        BuildingInfo.SetActive(true);

        RefreshRecipeList();
        RefreshOutputsUI();

        if (b.mc.BuildingIOManager.isConveyor && enableDebugSpawn)
            b.mc.BuildingIOManager.outputs[0].AddToSpawnQueue(testItemToSpawn, 0);
    }

    /// <summary>
    /// Event called when the building is deselected, sets the UI to inactive and calls the OnBuildingDeselected event
    /// </summary>
    public override void OnBuildingDeselected()
    {
        if (!FocusedBuilding)
            return;
        FocusedBuilding.mc.BuildingIOManager.DevisualizeAll();

        foreach (Building building in RegisteredBuildings)
        {
            building.mc.BuildingIOManager.DevisualizeAll();
        }

        base.OnBuildingDeselected();
        BuildingInfo.SetActive(false);
        IOSelectionPanel.SetActive(false);
    }

    /// <summary>
    /// Updates the Building UI with all of the correct information
    /// </summary>
    public override void OnBuildingUpdateUI()
    {
        base.OnBuildingUpdateUI();
        if (FocusedBuilding)
        {
            buildHealth.text = FocusedBuilding.Base.healthPercent.ToString();
            //onTime.text = FocusedBuilding.GetTimeForWS(WorkStateEnum.On).Duration().ToString();
            //idleTime.text = FocusedBuilding.GetTimeForWS(WorkStateEnum.Idle).Duration().ToString();
            //offTime.text = FocusedBuilding.GetTimeForWS(WorkStateEnum.Off).Duration().ToString();
            //itemInsideName.text = FocusedBuilding.BuildingIOManager.GetItemInsideName();
        }
    }

    /// <summary>
    /// Event called when the fix button is pressed, checks if the health is below optimal standards and tries to fix it
    /// </summary>
    public void OnFixButtonPressed()
    {
        if (FocusedBuilding.Base.healthPercent != 100)
        {
            FocusedBuilding.Fix();
        }
    }

    /// <summary>
    /// Sets the appropriate WorkState of the machine based off of button input
    /// </summary>
    /// <param name="buttonID">The button pressed</param>
    public void OnStateButtonPressed(int buttonID)
    {
        switch (buttonID)
        {
            case 1:
                FocusedBuilding.WorkState = WorkStateEnum.On;
                break;
            case 2:
                FocusedBuilding.WorkState = WorkStateEnum.Idle;
                break;
            case 3:
                FocusedBuilding.WorkState = WorkStateEnum.Off;
                break;
        }
    }

    #region IO Management

    private void RefreshRecipeList() 
    {
        recipeSelection.ClearOptions();

        recipeSelection.options.Add(new TMP_Dropdown.OptionData() { text = "None" });

        if (FocusedBuilding.mc.BuildingIOManager.isConveyor)
        {
            recipeSelection.value = 0;
            recipeSelection.RefreshShownValue();
            return;
        }

        if (!FocusedBuilding.mc.APM.CurrentRecipe)
        {
            recipeSelection.value = 0;
        }
        
        for (int i = 0; i < FocusedBuilding.mc.APM.recipePreset.AllowedRecipes.Count(); i++)
        {
            MachineRecipe recipe = FocusedBuilding.mc.APM.recipePreset.AllowedRecipes[i];
            recipeSelection.options.Add(new TMP_Dropdown.OptionData() { text = recipe.name });
            if (FocusedBuilding.mc.APM.CurrentRecipe == recipe)
                recipeSelection.value = i + 1;
        }

        recipeSelection.RefreshShownValue();

        recipeSelection.onValueChanged.AddListener(delegate { OnRecipeSelected(recipeSelection); });
    }

    private void OnRecipeSelected(TMP_Dropdown changed) 
    {
        if (changed.value == 0)
            FocusedBuilding.mc.APM.CurrentRecipe = null;
        else
            FocusedBuilding.mc.APM.CurrentRecipe = FocusedBuilding.mc.APM.recipePreset.AllowedRecipes[changed.value - 1];

        RefreshOutputsUI();
    }

    public void ShowIOSelectionUI() => RefreshOutputsUI(true);

    public void HideIOSelectionUI()
    {
        IOSelectionPanel.SetActive(false);
    }

    private void RefreshOutputsUI(bool showPanel = false)
    {
        if (!IsOutputSetupSupported())
            return;

        if (showPanel)
            IOSelectionPanel.SetActive(true);

        // visualize the outputsData from APM in the UI

        foreach (Transform field in outputSelectorFields)
        {
            Destroy(field.gameObject);
        }

        outputSelectorFields.Clear();

        for (int i = 0; i < FocusedBuilding.mc.APM.outputData.Count; i++)
        {
            KeyValuePair<MachineRecipe.OutputData, int> entry = FocusedBuilding.mc.APM.outputData.ElementAt(i);

            Transform fieldToAdd = Instantiate(OutputsSelector, OutputsParent);
            OutputSelector os = fieldToAdd.GetComponent<OutputSelector>();
            os.value = entry.Key;
            os.OutputID = entry.Value;
            os.itemNameText.text = entry.Key.item.name;
            outputSelectorFields.Add(fieldToAdd);
        }
    }
    #endregion

    private bool IsOutputSetupSupported() 
    {
        if (FocusedBuilding && FocusedBuilding.mc.APM && !FocusedBuilding.mc.Conveyor)
            return true;
        else
            return false;
    }
}
