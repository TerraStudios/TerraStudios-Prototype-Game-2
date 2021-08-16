//
// Developed by TerraStudios (https://github.com/TerraStudios)
//
// Copyright(c) 2020-2021 Konstantin Milev (konstantin890 | milev109@gmail.com)
// Copyright(c) 2020-2021 Yerti (UZ9)
//
// The following script has been written by either konstantin890 or Yerti (UZ9) or both.
// This file is covered by the GNU GPL v3 license. Read LICENSE.md for more information.
// Past NDA/MNDA and Confidential notices are revoked and invalid since no agreement took place. Read README.md for more information.
//

using System;
using System.Collections.Generic;
using BuildingModules;
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
        public Vector3 position;
        public Quaternion rotation;
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
        public List<BuildingSave> placedBuildings = new List<BuildingSave>();
    }
}
