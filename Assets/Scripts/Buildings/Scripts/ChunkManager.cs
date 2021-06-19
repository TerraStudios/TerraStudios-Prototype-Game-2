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
                building.correspondingMeshPrefab = reused;
                // Overwriting the current KVP so we can Destroy it later with OPM
                BuildingSystem.PlacedBuildings[chunk.chunkCoord][i] = new KeyValuePair<Building, GameObject>(building, reused);

                // IO Setup \\

                TerrainGenerator generator = TerrainGenerator.Instance;

                // Set position in the chunk it was placed in
                Debug.Log("Chunk log");
                int3 voxelPos = reused.transform.position.FloorToInt3();
                Vector3Int buildingSize = GridManager.Instance.GetBuildSize(reused.transform);

                // TODO: Move this to an enum
                VoxelType type = new VoxelType { isSolid = true };
                MachineSlaveVoxel slaveBlock = new MachineSlaveVoxel(type, building);
                //int3 localVoxelPos = generator.GetRelativeChunkPosition(voxelPos.x, voxelPos.y, voxelPos.z);

                //placedChunk.SetVoxelData(localVoxelPos.x, localVoxelPos.y, localVoxelPos.z, slaveBlock);

                for (int x = voxelPos.x - buildingSize.x + 1; x <= voxelPos.x; x++)
                {
                    for (int y = voxelPos.y; y < voxelPos.y + buildingSize.y; y++)
                    {
                        for (int z = voxelPos.z - buildingSize.z + 1; z <= voxelPos.z; z++)
                        {
                            if (!chunk.VoxelInsideChunk(x, y, z))
                            {
                                int3 localPos = generator.GetRelativeChunkPosition(x, y, z);
                                ChunkCoord coord = generator.GetChunkCoord(x, y, z);
                                generator.chunks[coord.x, coord.z].SetVoxelData(localPos.x, localPos.y, localPos.z, slaveBlock);
                            }
                            else
                            {
                                chunk.SetVoxelData(x, y, z, slaveBlock);
                            }

                        }
                    }
                }

                building.mc.buildingIOManager.LinkAll(); // figure out why they don't link. Voxel found it null.
                building.OnBuildingShow(); // consider moving bits of code from this function into Building.OnBuildingShow
            }
        }
    }

    /// <summary>
    /// Called when a Chunk got unloaded.
    /// </summary>
    /// <param name="chunkCoord">The ChunkCoord of the Chunk for which the action should be applied.</param>
    public void OnChunkUnloaded(ChunkCoord chunkCoord)
    {
        // Unload/disable all building mesh GameObjects in this chunk
        if (BuildingSystem.PlacedBuildings.Count != 0 && BuildingSystem.PlacedBuildings.ContainsKey(chunkCoord))
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
