//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System.Collections.Generic;
using BuildingManagement;
using BuildingModules;
using TerrainGeneration;
using TerrainTypes;
using Unity.Mathematics;
using UnityEngine;
using Utilities;

/// <summary>
/// Handles events that are called when a chunk is updated (loaded/unloaded).
/// </summary>
public class ChunkManager
{
    /// <summary>
    /// Called when a Chunk got loaded.
    /// </summary>
    /// <param name="chunk">The ChunkCoord of the Chunk for which the action should be applied.</param>
    public void OnChunkLoaded(Chunk chunk)
    {
        // Load all building mesh GameObjects in this chunk
        if (BuildingSystem.PlacedBuildings.Count != 0 && BuildingSystem.PlacedBuildings.ContainsKey(chunk.chunkCoord))
        {
            // Loop all placed buildings data in this chunk
            for (int i = 0; i < BuildingSystem.PlacedBuildings[chunk.chunkCoord].Count; i++)
            {
                KeyValuePair<Building, GameObject> kvp = BuildingSystem.PlacedBuildings[chunk.chunkCoord][i];

                Building building = kvp.Key;

                GameObject reused = ObjectPoolManager.Instance.ReuseObject(building.correspondingMeshPrefab.gameObject, building.meshData.pos, building.meshData.rot);
                building.InitMesh(reused.transform);

                // Overwriting the current KVP so we can Destroy it later with OPM
                BuildingSystem.PlacedBuildings[chunk.chunkCoord][i] = new KeyValuePair<Building, GameObject>(building, reused);

                // Set position in the chunk it was placed in
                int3 voxelPos = reused.transform.position.FloorToInt3();
                Vector3Int buildingSize = GridManager.Instance.GetBuildSize(reused.transform);

                // TODO: Move this to an enum
                VoxelType type = new VoxelType { isSolid = true };
                MachineSlaveVoxel slaveBlock = new MachineSlaveVoxel(type, building);

                chunk.SetVoxelRegion(voxelPos.x - buildingSize.x + 1, voxelPos.y,
                    voxelPos.z - buildingSize.z + 1, voxelPos.x, voxelPos.y + buildingSize.y, voxelPos.z, slaveBlock);

                building.mc.buildingIOManager.LinkAll();
                building.OnBuildingShow();
            }
        }
    }

    /// <summary>
    /// Called when a Chunk got unloaded.
    /// </summary>
    /// <param name="chunkCoord">The ChunkCoord of the Chunk for which the action should be applied.</param>
    public void OnChunkUnloaded(ChunkCoord chunkCoord, bool chunkRegenerate)
    {
        // Unload/disable all building mesh GameObjects in this chunk
        if (!chunkRegenerate && BuildingSystem.PlacedBuildings.Count != 0 && BuildingSystem.PlacedBuildings.ContainsKey(chunkCoord))
        {
            for (int i = 0; i < BuildingSystem.PlacedBuildings[chunkCoord].Count; i++)
            {
                KeyValuePair<Building, GameObject> kvp = BuildingSystem.PlacedBuildings[chunkCoord][i];

                if (kvp.Value != null)
                {
                    ObjectPoolManager.Instance.DestroyObject(kvp.Value);

                    // Make the corresponding mesh null so we don't somehow get an invalid one
                    BuildingSystem.PlacedBuildings[chunkCoord][i] = new KeyValuePair<Building, GameObject>(kvp.Key, null);

                    kvp.Key.OnBuildingHide(); // consider moving bits of code from this function into Building.OnBuildingHide
                }
            }
        }
    }
}
