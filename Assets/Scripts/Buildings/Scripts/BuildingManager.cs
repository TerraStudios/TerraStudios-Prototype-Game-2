using TMPro;
using UnityEngine;

public class BuildingManager : BuildingSystem
{
    public ItemData testItemToSpawn;
    [Header("UI Components")]
    public GameObject BuildingInfo;
    public TMP_Text buildHealth;
    public TMP_Text itemInsideName;

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

    private static BuildingManager s_Instance = null;

    public static BuildingManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(BuildingManager)) as BuildingManager;
            }

            return s_Instance;
        }
    }

    //! Probably has to be moved to BuildingSystem since this script should only handle UI
    public void Start()
    {
        //Create pools for each indicator
        ObjectPoolManager.instance.CreatePool(DirectionIndicator.gameObject, 80);
        ObjectPoolManager.instance.CreatePool(ArrowIndicator.gameObject, 8);
        ObjectPoolManager.instance.CreatePool(BrokenIndicator.gameObject, 50);
        ObjectPoolManager.instance.CreatePool(ErrorIndicator.gameObject, 50);
        ObjectPoolManager.instance.CreatePool(FixingIndicator.gameObject, 50);
    }

    //Static because the building manager doesn't have access to BuildingManager, and it doesn't make sense to put it in BuildingIOManager (multiple instances)

    /// <summary>
    /// Event called when the building is selected, sets the UI to active and calls the OnBuildingSelected event
    /// </summary>
    public override void OnBuildingSelected(Building b)
    {
        base.OnBuildingSelected(b);
        b.mc.BuildingIOManager.VisualizeAll();

        if (!b.mc.BuildingIOManager.isConveyor)
            foreach (Building building in RegisteredBuildings)
            {
                if (!building.mc.BuildingIOManager.isConveyor)
                    building.mc.BuildingIOManager.VisualizeAll();
            }

        BuildingInfo.SetActive(true);
        b.mc.BuildingIOManager.outputs[0].SpawnItemObj(testItemToSpawn);
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
    }

    /// <summary>
    /// Updates the Building UI with all of the correct information
    /// </summary>
    public override void OnBuildingUpdateUI()
    {
        base.OnBuildingUpdateUI();
        if (FocusedBuilding)
        {
            buildHealth.text = FocusedBuilding.healthPercent.ToString();
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
        if (FocusedBuilding.healthPercent != 100)
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
}
