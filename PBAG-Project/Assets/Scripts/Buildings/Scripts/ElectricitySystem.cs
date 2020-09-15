using UnityEngine;

public class ElectricitySystem : MonoBehaviour
{
    [Header("Components")]
    public BuildingManager BuildingManager;

    public int GetTotalElectricityUsed(WorkStateEnum ws)
    {
        int toReturn = 0;

        if (!GameManager.profile.enableElectricityCalculations)
            return 0;

        foreach (Building b in BuildingSystem.RegisteredBuildings)
        {
            toReturn += b.GetUsedElectricity(ws);
        }
        return toReturn;
    }
}
