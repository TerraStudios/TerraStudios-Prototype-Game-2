using BuildingModules;
using System;
using System.Collections.Generic;
using TerrainGeneration;
using UnityEngine;

namespace SaveSystem
{
    /// <summary>
    /// Contains data to save for each building.
    /// </summary>
    [Serializable]
    public class BuildingSave
    {
#pragma warning disable CA2235 // Uses serialization surrogate
        public ChunkCoord chunkCoord;
        public Quaternion rotation;
#pragma warning restore CA2235 // End serialization surrogate usage
        public BuildingBase building;
        public string prefabLocation;

        public Transform GetObj()
        {
            return Resources.Load<Transform>(prefabLocation);
        }
    }

    /// <summary>
    /// Contains data to save about the world
    /// </summary>
    [Serializable]
    public class WorldSaveData
    {
        public List<BuildingSave> placedBuildings = new List<BuildingSave>();
    }
}