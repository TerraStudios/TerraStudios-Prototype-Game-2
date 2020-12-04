using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// This class is the highest level of the <c>BuildingSystem</c>.
/// It handles connections with the UI along with references to GameObjects.
/// </summary>
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
    public Transform InputsParent;
    public Transform InputsSelector;
    public Transform OutputsParent;
    public Transform OutputsSelector;
    private List<Transform> inputSelectorFields = new List<Transform>();
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

        ClearRegisteredBuildings();
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
        RefreshIOUI();

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

        for (int i = 0; i < FocusedBuilding.mc.APM.allowedRecipes.Count; i++)
        {
            MachineRecipe recipe = FocusedBuilding.mc.APM.allowedRecipes[i];
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
            FocusedBuilding.mc.APM.CurrentRecipe = FocusedBuilding.mc.APM.allowedRecipes[changed.value - 1];

        RefreshIOUI();
    }

    public void ShowIOSelectionUI() => RefreshIOUI(true);

    public void HideIOSelectionUI()
    {
        IOSelectionPanel.SetActive(false);
    }

    private void RefreshIOUI(bool showPanel = false)
    {
        if (!IsOutputSetupSupported())
            return;

        if (showPanel)
            IOSelectionPanel.SetActive(true);

        // visualize the inputData from APM to the UI

        foreach (Transform field in inputSelectorFields)
        {
            Destroy(field.gameObject);
        }

        inputSelectorFields.Clear();

        for (int i = 0; i < FocusedBuilding.mc.APM.inputData.Count; i++)
        {
            KeyValuePair<MachineRecipe.InputData, int> entry = FocusedBuilding.mc.APM.inputData.ElementAt(i);

            Transform fieldToAdd = Instantiate(InputsSelector, InputsParent);
            InputSelector os = fieldToAdd.GetComponent<InputSelector>();
            os.value = entry.Key;
            os.InputID = entry.Value;
            os.itemNameText.text = entry.Key.item.name;
            inputSelectorFields.Add(fieldToAdd);

            if (!FocusedBuilding.mc.APM.CurrentRecipe.allowPlayerInputsConfiguration)
                os.button.interactable = false;
        }

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

            if (!FocusedBuilding.mc.APM.CurrentRecipe.allowPlayerOutputsConfiguration)
                os.button.interactable = false;
        }
    }
    #endregion

    private bool IsOutputSetupSupported()
    {
        return FocusedBuilding && FocusedBuilding.mc.APM && !FocusedBuilding.mc.Conveyor;
    }
}
