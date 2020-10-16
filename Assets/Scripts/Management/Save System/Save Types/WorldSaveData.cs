using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingSave
{
#pragma warning disable CA2235 // Uses serialization surrogate
    public Vector3 location;
    public Quaternion rotation;
#pragma warning restore CA2235 // End serialization surrogate usage
    public BuildingBase building;
    public string prefabLocation;

    public Transform GetObj() 
    {
        return Resources.Load<Transform>(prefabLocation);
    }
}

[Serializable]
public class WorldSaveData
{
    public List<BuildingSave> PlacedBuildings = new List<BuildingSave>();
}
