using BuildingModules;
using TMPro;
using UnityEngine;

namespace BuildingManagers
{
    /// <summary>
    /// Highest level class for the Electricity System.
    /// Handles UI connections with the code.
    /// </summary>
    public class ElectricityManager : ElectricitySystem
    {
        [Header("UI Components")]
        public TMP_Text totalWattsForWork;
        public TMP_Text totalWattsForIdle;

        public void UpdateWatts()
        {
            totalWattsForWork.text = GetTotalElectricityUsed(WorkStateEnum.On).ToString() + " w";
            totalWattsForIdle.text = GetTotalElectricityUsed(WorkStateEnum.Idle).ToString() + " w";
        }
    }
}
