using UnityEngine;

public class ElectricitySystem : MonoBehaviour
{
    [Header("Components")]
    public BuildingManager BuildingManager;

    public int GetTotalElectricityUsed(WorkStateEnum ws)
    {
        int toReturn = 0;
        foreach (Building b in BuildingManager.RegisteredBuildings)
        {
            toReturn += b.GetUsedElectricity(ws);
        }
        return toReturn;
    }
}
