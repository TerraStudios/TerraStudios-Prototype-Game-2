using TMPro;
using UnityEngine;

public class BuildingManager : BuildingSystem
{
    [Header("UI Components")]
    public GameObject BuildingInfo;
    public TMP_Text buildHealth;
    public TMP_Text onTime;
    public TMP_Text idleTime;
    public TMP_Text offTime;
    public TMP_Text itemInsideName;


    /// <summary>
    /// Event called when the building is selected, sets the UI to active and calls the OnBuildingSelected event
    /// </summary>
    public override void OnBuildingSelected(Building b)
    {
        base.OnBuildingSelected(b);
        BuildingInfo.SetActive(true);
    }


    /// <summary>
    /// Event called when the building is deselected, sets the UI to inactive and calls the OnBuildingDeselected event
    /// </summary>
    public override void OnBuildingDeselected()
    {
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
            onTime.text = FocusedBuilding.GetTimeForWS(WorkStateEnum.On).Duration().ToString();
            idleTime.text = FocusedBuilding.GetTimeForWS(WorkStateEnum.Idle).Duration().ToString();
            offTime.text = FocusedBuilding.GetTimeForWS(WorkStateEnum.Off).Duration().ToString();
            itemInsideName.text = FocusedBuilding.BuildingIOManager.GetItemInsideName();
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
