using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleConnector : MonoBehaviour
{
    [Header("Required")]
    [Tooltip("The building the ModuleConnector is attached to")]
    public Building Building;
    
    [Header("Optional")]
    [Tooltip("The BuildingIOManager script of a Building")]
    public BuildingIOManager BuildingIOManager;
    [Tooltip("The APM script of a Building")]
    public APM APM;
}
