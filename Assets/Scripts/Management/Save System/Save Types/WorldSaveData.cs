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
        public ChunkCoord chunkCoord;
#pragma warning disable CA2235 // Uses serialization surrogate
        public Vector3 position;
        public Quaternion rotation;
#pragma warning restore CA2235 // End serialization surrogate usage
        public BuildingBase building;
        public string scriptPrefabPath;

        public Transform GetScriptObj()
        {
            return Resources.Load<Transform>(scriptPrefabPath);
        }

        public Transform GetMeshObj()
        {
            return Resources.Load<Transform>(scriptPrefabPath + "_Mesh");
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