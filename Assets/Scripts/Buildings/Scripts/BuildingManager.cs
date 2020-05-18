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

    public override void OnBuildingSelected(Building b)
    {
        base.OnBuildingSelected(b);
        BuildingInfo.SetActive(true);
    }

    public override void OnBuildingDeselected()
    {
        base.OnBuildingDeselected();
        BuildingInfo.SetActive(false);
    }

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

    public void OnFixButtonPressed()
    {
        if (FocusedBuilding.healthPercent != 100)
        {
            FocusedBuilding.Fix();
        }
    }

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
