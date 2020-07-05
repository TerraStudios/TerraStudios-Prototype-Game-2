using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleConnector : MonoBehaviour
{
    [Header("Required")]
    public Building Building;
    
    [Header("Optional")]
    public BuildingIOManager BuildingIOManager;
    public APM APM;
}
