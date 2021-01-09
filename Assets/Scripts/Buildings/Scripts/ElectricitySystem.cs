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

            foreach (KeyValuePair<Building, GameObject> kvp in BuildingSystem.RegisteredBuildings.Keys)
            {
                toReturn += kvp.Key.GetUsedElectricity(ws);
            }
            return toReturn;
        }
    }
}
