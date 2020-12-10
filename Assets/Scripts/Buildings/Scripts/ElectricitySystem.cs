using UnityEngine;
using BuildingModules;

namespace BuildingManagers
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

            if (!GameManager.Profile.enableElectricityCalculations)
                return 0;

            foreach (Building b in BuildingSystem.RegisteredBuildings)
            {
                toReturn += b.GetUsedElectricity(ws);
            }
            return toReturn;
        }
    }
}
