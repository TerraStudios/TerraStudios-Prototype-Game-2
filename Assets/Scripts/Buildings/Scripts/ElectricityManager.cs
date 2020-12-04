using TMPro;
using UnityEngine;

/// <summary>
/// Highest level class for the Electricity System.
/// Handles UI connections with the code.
/// </summary>
public class ElectricityManager : ElectricitySystem
{
    [Header("UI Components")]
    public TMP_Text TotalWattsForWork;
    public TMP_Text TotalWattsForIdle;

    public void UpdateWatts()
    {
        TotalWattsForWork.text = GetTotalElectricityUsed(WorkStateEnum.On).ToString() + " w";
        TotalWattsForIdle.text = GetTotalElectricityUsed(WorkStateEnum.Idle).ToString() + " w";
    }
}
