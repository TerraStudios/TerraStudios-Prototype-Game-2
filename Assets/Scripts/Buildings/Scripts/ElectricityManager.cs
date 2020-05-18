using TMPro;
using UnityEngine;

public class ElectricityManager : ElectricitySystem
{
    [Header("UI Components")]
    public TMP_Text TotalWattsForWork;
    public TMP_Text TotalWattsForIdle;

    private void Update()
    {
        TotalWattsForWork.text = GetTotalElectricityUsed(WorkStateEnum.On).ToString() + " w";
        TotalWattsForIdle.text = GetTotalElectricityUsed(WorkStateEnum.Idle).ToString() + " w";
    }
}
