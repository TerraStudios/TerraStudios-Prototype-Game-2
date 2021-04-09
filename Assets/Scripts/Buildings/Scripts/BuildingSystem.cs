﻿//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using BuildingModules;
using EconomyManagement;
using Player;
using SaveSystem;
using System.Collections.Generic;
using System.Linq;
using TerrainGeneration;
using TerrainTypes;
using TimeSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace BuildingManagement
{
    /// <summary>
    /// Class that contains all necessary data for registering a Building.
    /// </summary>
    public class RegisterBuildingData
    {
        /// <summary>
        /// The Building script of the Building Script GO.
        /// </summary>
        public Building building;

        /// <summary>
        /// The Mesh GameObject of the Building Mesh GO.
        /// </summary>
        public Transform buildingMesh;

        /// <summary>
        /// The Mesh Prefab of the Building.
        /// </summary>
        public Transform buildingMeshPrefab;

        /// <summary>
        /// The chunk coordinate where the building is placed.
        /// </summary>
        public ChunkCoord chunkCoord;

        /// <summary>
        /// Whether the Building should be initialized with Building.Init. Make false if loading buildings from save.
        /// </summary>
        public bool register = true;

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
    }

    /// <summary>
    /// This class acts like a layer between the <c>GridManager</c> and the <c>BuildingSystem</c>.
    /// Handles registering buildings as well as loading and setting them up.
    /// </summary>
    public class BuildingSystem : MonoBehaviour
    {
        [HideInInspector] public Building focusedBuilding;
        public LayerMask ignoreFocusLayers;
        public int meshPoolSize;

        [Header("Components")]
        public TimeManager timeManager;

        public Camera mainCamera;
        public GridManager gridManager;
        public EconomyManager economyManager;

        /// <summary>
        /// A dictionary that stores the following things:
        /// Key - Coordinates of the chunk where the Building is placed.
        /// Value - List of KeyValuePairs (Buildings in ChunkCoord) containing the Building (Script Prefab) as a key and GameObject (Mesh Prefab) as a value.
        /// </summary>
        public static Dictionary<ChunkCoord, List<KeyValuePair<Building, GameObject>>> PlacedBuildings = new Dictionary<ChunkCoord, List<KeyValuePair<Building, GameObject>>>();

        public GameObject buildingScriptParent;
        public GameObject buildingMeshParent;

        //0 = apm1
        //1 = apm2
        //2 = apm3
        //3 = conveyor
        public string[] buildingLocations;

        /// <summary>
        /// Executes necessary logic for newly placed buildings.
        /// </summary>
        /// <param name="data">Special data class needed for setting up a building.</param>
        public static void RegisterBuilding(RegisterBuildingData data)
        {
            KeyValuePair<Building, GameObject> toRegister = new KeyValuePair<Building, GameObject>(data.building, data.buildingMesh.gameObject);

            if (data.valueListID == -1)
                PlacedBuildings[data.chunkCoord].Add(toRegister);
            else
                PlacedBuildings[data.chunkCoord].Insert(data.valueListID, toRegister);

            int keyListID = PlacedBuildings.Keys.ToList().IndexOf(data.chunkCoord);
            int valueListID = PlacedBuildings[data.chunkCoord].IndexOf(toRegister);

            if (data.register)
            {
                BuildingSave toSave = new BuildingSave()
                {
                    //keyID = key,
                    valueListID = valueListID,
                    chunkCoord = data.chunkCoord,
                    position = data.building.correspondingMesh.position,
                    rotation = data.building.correspondingMesh.rotation,
                    building = data.building.bBase,
                    meshData = data.building.meshData,
                    scriptPrefabPath = data.building.scriptPrefabLocation
                };

                GameSave.current.worldSaveData.placedBuildings.Add(toSave);
            }

            TerrainGenerator generator = TerrainGenerator.Instance;

            // Set position in the chunk it was placed in

            int3 voxelPos = data.buildingMesh.position.FloorToInt3();
            Chunk placedChunk = generator.currentChunks[data.chunkCoord];
            Vector3Int buildingSize = data.building.meshData.size;

            // TODO: Move this to an enum
            VoxelType type = new VoxelType { isSolid = true };
            MachineSlaveVoxel slaveBlock = new MachineSlaveVoxel(type, keyListID, valueListID);
            //int3 localVoxelPos = generator.GetRelativeChunkPosition(voxelPos.x, voxelPos.y, voxelPos.z);

            //placedChunk.SetVoxelData(localVoxelPos.x, localVoxelPos.y, localVoxelPos.z, slaveBlock);

            for (int x = voxelPos.x - buildingSize.x + 1; x <= voxelPos.x; x++)
            {
                for (int y = voxelPos.y; y < voxelPos.y + buildingSize.y; y++)
                {
                    for (int z = voxelPos.z - buildingSize.z + 1; z <= voxelPos.z; z++)
                    {
                        if (!placedChunk.VoxelInsideChunk(x, y, z))
                        {
                            int3 localPos = generator.GetRelativeChunkPosition(x, y, z);
                            generator.currentChunks[generator.GetChunkCoord(x, y, z)].SetVoxelData(localPos.x, localPos.y, localPos.z, slaveBlock);
                        }
                        else
                        {
                            placedChunk.SetVoxelData(x, y, z, slaveBlock);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Executes necessary logic for unregistering placed buildings.
        /// </summary>
        /// <param name="b">The Building component of the building to unregister.</param>
        public static void UnRegisterBuilding(Building b)
        {
            // Remove List entry of Building b found in the KVP List in RegisteredBuildings.Values
            foreach (List<KeyValuePair<Building, GameObject>> kvp in PlacedBuildings.Values)
            {
                kvp.Remove(kvp.Find(kvp => kvp.Key == b));
            }

            GameSave.current.worldSaveData.placedBuildings.Where(bSave => bSave.building == b.bBase);
        }

        /// <summary>
        /// Clears the list of registered buildings.
        /// </summary>
        public void ClearRegisteredBuildings() { PlacedBuildings.Clear(); }

        /// <summary>
        /// Main update loop for the BuildingSystem, refreshes the UI with OnBuildingUpdateUI
        /// </summary>
        private void Update()
        {
            OnBuildingUpdateUI();
        }

        /// <summary>
        /// Fills the PlacedBuilding dictionary with all ChunkCoords available.
        /// </summary>
        public void PrePopulatePlacedBuildings()
        {
            // It's safe to prepopulate here since it's executed before buildings are loaded from the save.
            foreach (ChunkCoord coord in TerrainGenerator.Instance.GetAllChunkCoords())
            {
                PlacedBuildings.Add(coord, new List<KeyValuePair<Building, GameObject>>());
            }
        }

        /// <summary>
        /// Loads all BuildingSave data from the current GameSave.
        /// </summary>
        public void LoadAllBuildingsFromSave()
        {
            foreach (BuildingSave save in GameSave.current.worldSaveData.placedBuildings)
            {
                Transform buildingGO = Instantiate(save.GetScriptObj().gameObject, Vector3.zero, Quaternion.identity).transform;

                Building building = buildingGO.GetComponent<Building>();

                if (building == null) Debug.Log("Building null");
                //if (building.GetMeshObj(building.scriptPrefabLocation) == null) Debug.Log("Null mesh obj");
                if (save == null) Debug.Log("Save was null");
                if (save.position == null) Debug.Log("Save position was null");
                if (save.rotation == null) Debug.Log("Save rotation was null");

                GameObject meshPrefab = save.GetMeshObj().gameObject;

                Transform meshGO = ObjectPoolManager.Instance.ReuseObject(meshPrefab, save.position, save.rotation).transform;

                //buildingGO.parent = buildingScriptParent.transform;
                //meshGO.parent = buildingMeshParent.transform;

                ChunkCoord chunkCoord = save.chunkCoord;

                building.bBase = save.building;
                building.meshData = save.meshData;

                RegisterBuildingData data = new RegisterBuildingData()
                {
                    keyID = save.keyID,
                    valueListID = save.valueListID,
                    building = building,
                    buildingMesh = meshGO,
                    buildingMeshPrefab = meshPrefab.transform,
                    chunkCoord = chunkCoord,
                    register = false
                };

                SetUpBuilding(data);
            }
        }

        public void PoolAllBuildingMeshes()
        {
            for (int i = 0; i < buildingLocations.Length; i++)
            {
                ObjectPoolManager.Instance.CreatePool(Resources.Load<Transform>(buildingLocations[i] + "_Mesh").gameObject, meshPoolSize);
            }
            Debug.Log("Pooled all Building Meshes, in total: " + buildingLocations.Length);
        }

        /// <summary>
        /// Retrieves Script and Mesh GameObjects for the corresponding asset location.
        /// </summary>
        /// <param name="resourcesID">The ID of the Resources locations available in the buildingLocations list.</param>
        /// <returns>Script and Mesh GameObjects for the corresponding Building.</returns>
        public KeyValuePair<Building, Transform> GetBuildingFromLocation(int resourcesID)
        {
            string resourcesLocation = buildingLocations[resourcesID];
            Transform scriptGO = Resources.Load<Transform>(resourcesLocation);

            //Transform meshGO = scriptGO.GetChild(0);
            Transform meshGO = Resources.Load<Transform>(resourcesLocation + "_Mesh");

            Building b = scriptGO.GetComponent<Building>();
            b.scriptPrefabLocation = resourcesLocation;
            return new KeyValuePair<Building, Transform>(b, meshGO);
        }

        /// <summary>
        /// Initializes all of the needed data for the building in question.
        /// </summary>
        /// <param name="data">Special data class needed for setting up a building.</param>
        public void SetUpBuilding(RegisterBuildingData data)
        {
            //TODO: THESE HAVE BEEN COMMENTED OUT AS OF 2/13/2021, TO BE CHANGED?
            //b.transform.parent = buildingScriptParent.transform;
            //t.transform.parent = buildingMeshParent.transform;
            data.building.timeManager = timeManager;
            data.building.economyManager = economyManager;
            data.building.correspondingMeshPrefab = data.buildingMeshPrefab.gameObject;
            RegisterBuilding(data);
            data.building.Init(data.buildingMesh, !data.register);

            if (data.building.mc.buildingIOManager.isConveyor)
            {
                ConveyorManager.Instance.conveyors.Add(data.building.GetComponent<Conveyor>());
            }
        }

        /// <summary>
        /// Checks if a hit finds a building and if necessary calls the OnBuildingSelected event
        /// </summary>
        /// <param name="mousePos">The Vector3 to start the Raycast from</param>
        public void CheckForHit(Vector3 mousePos)
        {
            if (EventSystem.current.IsPointerOverGameObject() || RemoveSystem.instance.removeModeEnabled)
                return;

            Ray ray = mainCamera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~ignoreFocusLayers))
            {
                Building b = hit.transform.GetComponent<Building>();
                if (b)
                {
                    if (b.isSetUp)
                    {
                        if (focusedBuilding && !(b.Equals(focusedBuilding)))
                        {
                            OnBuildingDeselected();
                        }
                        OnBuildingSelected(b);
                    }
                }
                else
                {
                    if (focusedBuilding)
                    {
                        OnBuildingDeselected();
                    }
                }
            }
            else
            {
                if (focusedBuilding)
                {
                    OnBuildingDeselected();
                }
            }
        }

        public virtual void OnBuildingSelected(Building b) => focusedBuilding = b;

        public virtual void OnBuildingDeselected() => focusedBuilding = null;

        public virtual void OnBuildingUpdateUI() { }
    }
}
