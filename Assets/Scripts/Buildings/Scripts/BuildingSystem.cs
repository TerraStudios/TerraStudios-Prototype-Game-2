//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System.Collections.Generic;
using System.Linq;
using BuildingModules;
using EconomyManagement;
using Player;
using SaveSystem;
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
    }

    /// <summary>
    /// This class acts like a layer between the <c>GridManager</c> and the <c>BuildingSystem</c>.
    /// Handles registering buildings as well as loading and setting them up.
    /// </summary>
    public class BuildingSystem : MonoBehaviour
    {
        [HideInInspector] public Building focusedBuilding;
        public LayerMask buildingFocusLayer;
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
            if (PlacedBuildings.ContainsKey(data.chunkCoord))
            {
                PlacedBuildings[data.chunkCoord].Add(new KeyValuePair<Building, GameObject>(data.building, data.buildingMesh.gameObject));
            }
            else
            {
                PlacedBuildings.Add(data.chunkCoord, new List<KeyValuePair<Building, GameObject>> { new KeyValuePair<Building, GameObject>(data.building, data.buildingMesh.gameObject) });
            }

            if (data.register)
            {
                BuildingSave toSave = new BuildingSave()
                {
                    chunkCoord = data.chunkCoord,
                    position = data.building.correspondingMesh.position,
                    rotation = data.building.correspondingMesh.rotation,
                    building = data.building.bBase,
                    meshData = data.building.meshData,
                    scriptPrefabPath = data.building.scriptPrefabLocation
                };

                GameSave.current.worldSaveData.placedBuildings.Add(toSave);
            }
        }

        /// <summary>
        /// Executes necessary logic for unregistering placed buildings.
        /// </summary>
        /// <param name="b">The Building component of the building to unregister.</param>
        public static void UnRegisterBuilding(Building b)
        {
            // Remove List entry of Building b found in the KVP List in RegisteredBuildings.Values
            foreach (var chunkKvp in PlacedBuildings)
            {
                foreach (var kvp in chunkKvp.Value)
                {
                    if (kvp.Key == b)
                    {
                        chunkKvp.Value.Remove(kvp);

                        int3 voxelPos = b.meshData.pos.FloorToInt3();
                        Vector3Int buildingSize = b.meshData.size;
                        TerrainGenerator generator = TerrainGenerator.Instance;
                        Chunk chunk = generator.currentChunks[chunkKvp.Key];


                        for (int x = voxelPos.x - buildingSize.x + 1; x <= voxelPos.x; x++)
                        {
                            for (int y = voxelPos.y; y < voxelPos.y + buildingSize.y; y++)
                            {
                                for (int z = voxelPos.z - buildingSize.z + 1; z <= voxelPos.z; z++)
                                {
                                    if (!chunk.VoxelInsideChunk(x, y, z))
                                    {
                                        int3 localPos = generator.GetRelativeChunkPosition(x, y, z);
                                        generator.currentChunks[generator.GetChunkCoord(x, y, z)]
                                            .SetVoxelData(localPos.x, localPos.y, localPos.z, null);
                                    }
                                    else
                                    {
                                        chunk.SetVoxelData(x, y, z, null);
                                    }

                                }
                            }
                        }

                        return;
                    }
                }
            }

            BuildingSave buildingToRemove = GameSave.current.worldSaveData.placedBuildings.Single(bSave => bSave.building == b.bBase);
            GameSave.current.worldSaveData.placedBuildings.Remove(buildingToRemove);
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
        /// Loads all BuildingSave data from the current GameSave.
        /// </summary>
        public void LoadAllBuildingsFromSave()
        {
            foreach (BuildingSave save in GameSave.current.worldSaveData.placedBuildings)
            {
                Transform buildingGO = Instantiate(save.GetScriptObj().gameObject, Vector3.zero, Quaternion.identity).transform;

                Building building = buildingGO.GetComponent<Building>();

                if (building == null) Debug.Log("Building null");

                GameObject meshPrefab = save.GetMeshObj().gameObject;

                Transform meshGO = ObjectPoolManager.Instance.ReuseObject(meshPrefab, save.position, save.rotation).transform;

                ChunkCoord chunkCoord = save.chunkCoord;

                building.bBase = save.building;
                building.meshData = save.meshData;

                RegisterBuildingData data = new RegisterBuildingData()
                {
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
        /// Retrives a list of Script and Mesh GameObjects for all asset locations.
        /// </summary>
        /// <returns>A list of all Script and Mesh GameObjects available.</returns>
        public List<KeyValuePair<Building, Transform>> GetAllBuildings()
        {
            List<KeyValuePair<Building, Transform>> toReturn = new List<KeyValuePair<Building, Transform>>();

            for (int i = 0; i < buildingLocations.Length; i++)
            {
                toReturn.Add(GetBuildingFromLocation(i));
            }

            return toReturn;
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

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, buildingFocusLayer))
            {
                Building b = (TerrainGenerator.Instance.GetVoxel(hit.transform.position.FloorToInt3()) as MachineSlaveVoxel).controller;

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

        public virtual void OnBuildingSelected(Building b) => focusedBuilding = b;

        public virtual void OnBuildingDeselected() => focusedBuilding = null;

        public virtual void OnBuildingUpdateUI() { }
    }
}
