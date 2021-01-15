//
// Developped by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement.
// All of the contents of this script are Confidential. Distributing or using them for your own needs is prohibited.
// Destroy the file immediately if you are not one of the parties involved.
//

using BuildingModules;
using CoreManagement;
using UnityEngine;
using System.Collections.Generic;

namespace BuildingManagement
{
    /// <summary>
    /// Used to gather and generate electricity data for the Building.
    /// </summary>
    public class ElectricitySystem : MonoBehaviour
    {
        [Header("Components")]
        public BuildingManager buildingManager;

        public int GetTotalElectricityUsed(WorkStateEnum ws)
        {
            int toReturn = 0;

            if (!GameManager.Instance.CurrentGameProfile.enableElectricityCalculations)
                return 0;

            foreach (List<KeyValuePair<Building, GameObject>> kvp in BuildingSystem.PlacedBuildings.Values)
                foreach (KeyValuePair<Building, GameObject> buildingKVP in kvp)
                {
                    toReturn += buildingKVP.Key.GetUsedElectricity(ws);
                }
            return toReturn;
        }
    }
}
