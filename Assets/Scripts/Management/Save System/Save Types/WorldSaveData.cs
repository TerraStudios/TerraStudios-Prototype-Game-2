//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

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
        /// <summary>
        /// Used for specifying the index of the key with which the Building should be registered in PlacedBuildings. 
        /// Used for recreating the original PlacedBuildings Dictionary.
        /// -1 to ignore (default)
        /// </summary>
        public int keyID = -1;

        /// <summary>
        /// Used for specifying the index of the value with which the Building should be registered in PlacedBuildings. 
        /// Used for recreating the original PlacedBuildings Dictionary.
        /// -1 to ignore (default)
        /// </summary>
        public int valueListID = -1;

        public ChunkCoord chunkCoord;
#pragma warning disable CA2235 // Uses serialization surrogate
        public Vector3 position;
        public Quaternion rotation;
#pragma warning restore CA2235 // End serialization surrogate usage
        public BuildingBase building;
        public MeshData meshData;
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
        public Dictionary<ChunkCoord, Voxel[]> voxelData = new Dictionary<ChunkCoord, Voxel[]>();
        public List<BuildingSave> placedBuildings = new List<BuildingSave>();
    }
}
