using System.Collections;
using System.Collections.Generic;
using BuildingManagement;
using BuildingModules;
using TerrainGeneration;
using UnityEngine;
using Utilities;

public class ChunkManager
{
    public void OnChunkLoaded(ChunkCoord chunkCoord)
    {
        // Load all building mesh GameObjects in this chunk
        if (BuildingSystem.PlacedBuildings.Count != 0 && BuildingSystem.PlacedBuildings.ContainsKey(chunkCoord))
        {
            // Loop all placed buildings data in this chunk
            for (int i = 0; i < BuildingSystem.PlacedBuildings[chunkCoord].Count; i++)
            {
                KeyValuePair<Building, GameObject> kvp = BuildingSystem.PlacedBuildings[chunkCoord][i];

                Building building = kvp.Key;
                //GameObject mesh = list[i].Value;
                GameObject go = building.GetMeshObj(kvp.Key.scriptPrefabLocation).gameObject;
                Vector3 pos = kvp.Key.meshData.pos;
                Quaternion rot = kvp.Key.meshData.rot;

                GameObject reused = ObjectPoolManager.Instance.ReuseObject(go, pos, rot);
                building.correspondingMesh = reused.transform;
                // Overwriting the current KVP so we can Destroy it later with OPM
                BuildingSystem.PlacedBuildings[chunkCoord][i] = new KeyValuePair<Building, GameObject>(building, reused);
            }

            Debug.Log("Enabled " + BuildingSystem.PlacedBuildings[chunkCoord].Count + " meshes!");
        }
    }

    public void OnChunkUnloaded(ChunkCoord chunkCoord)
    {
        // Unload/disable all building mesh GameObjects in this chunk
        if (BuildingSystem.PlacedBuildings.Count != 0 && BuildingSystem.PlacedBuildings.ContainsKey(chunkCoord))
        {
            Debug.Log("Destroying " + BuildingSystem.PlacedBuildings[chunkCoord].Count + " meshes!");

            for (int i = 0; i < BuildingSystem.PlacedBuildings[chunkCoord].Count; i++)
            {
                KeyValuePair<Building, GameObject> kvp = BuildingSystem.PlacedBuildings[chunkCoord][i];

                if (kvp.Value != null)
                {
                    ObjectPoolManager.Instance.DestroyObject(kvp.Value);
                    // Make the corresponding mesh null so we don't somehow get an invalid one
                    BuildingSystem.PlacedBuildings[chunkCoord][i] = new KeyValuePair<Building, GameObject>(kvp.Key, null);
                }
            }
        }
    }
}
