//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement.
// All of the contents of this script are Confidential. Distributing or using them for your own needs is prohibited.
// Destroy the file immediately if you are not one of the parties involved.
//

using BuildingModules;
using TMPro;
using UnityEngine;

namespace BuildingManagement
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
