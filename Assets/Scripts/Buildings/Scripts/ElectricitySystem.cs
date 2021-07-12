//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System.Collections.Generic;
using BuildingModules;
using CoreManagement;
using UnityEngine;

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
