//
// Developed by TerraStudios (https://github.com/TerraStudios)
//
// Copyright(c) 2020-2021 Konstantin Milev (konstantin890 | milev109@gmail.com)
// Copyright(c) 2020-2021 Yerti (UZ9)
//
// The following script has been written by either konstantin890 or Yerti (UZ9) or both.
// This file is covered by the GNU GPL v3 license. Read LICENSE.md for more information.
// Past NDA/MNDA and Confidential notices are revoked and invalid since no agreement took place. Read README.md for more information.
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
