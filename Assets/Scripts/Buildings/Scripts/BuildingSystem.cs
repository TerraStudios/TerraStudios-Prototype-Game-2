//
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
using TimeSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace BuildingManagement
{
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
        /// <param name="chunkCoord">The chunk coordinate where the building is placed.</param>
        /// <param name="b">The Building script of the Building Script GO.</param>
        /// <param name="meshGO">The Mesh GameObject of the Building Mesh GO.</param>
        /// <param name="save">Whether the Building should be saved in the GameSave<. Make false if loading buildings from save to avoid stack overflow.</param>
        public static void RegisterBuilding(ChunkCoord chunkCoord, Building b, GameObject meshGO, bool save = true)
        {
            if (PlacedBuildings.ContainsKey(chunkCoord))
            {
                PlacedBuildings[chunkCoord].Add(new KeyValuePair<Building, GameObject>(b, meshGO));
            }
            else
            {
                PlacedBuildings.Add(chunkCoord, new List<KeyValuePair<Building, GameObject>> { new KeyValuePair<Building, GameObject>(b, meshGO) });
            }

            if (save)
            {
                BuildingSave toSave = new BuildingSave()
                {
                    chunkCoord = chunkCoord,
                    position = b.correspondingMesh.position,
                    rotation = b.correspondingMesh.rotation,
                    building = b.bBase,
                    scriptPrefabPath = b.scriptPrefabLocation
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
        /// Loads all BuildingSave data from the current GameSave.
        /// </summary>
        public void LoadAllBuildingsFromSave()
        {
            foreach (BuildingSave save in GameSave.current.worldSaveData.placedBuildings)
            {
                Transform buildingGO = Instantiate(save.GetScriptObj().gameObject, Vector3.zero, Quaternion.identity).transform;

                Building building = buildingGO.GetComponent<Building>();

                if (building == null) Debug.Log("Building null");
                if (building.GetMeshObj(building.scriptPrefabLocation) == null) Debug.Log("Null mesh obj");
                if (save == null) Debug.Log("Save was null");
                if (save.position == null) Debug.Log("Save position was null");
                if (save.rotation == null) Debug.Log("Save rotation was null");

                Transform meshGO = Instantiate(building.GetMeshObj(building.scriptPrefabLocation).gameObject, save.position, save.rotation).transform;

                //buildingGO.parent = buildingScriptParent.transform;
                //meshGO.parent = buildingMeshParent.transform;

                ChunkCoord chunkCoord = save.chunkCoord;
                building.bBase = save.building;
                SetUpBuilding(building, meshGO, chunkCoord, false);
            }
        }

        public void PoolAllBuildingMeshes()
        {
            for (int i = 0; i < buildingLocations.Length; i++)
            {
                ObjectPoolManager.Instance.CreatePool(Resources.Load<Transform>(buildingLocations[i] + "_Mesh").gameObject, meshPoolSize);
            }
            Debug.Log("Pooled all Building Meshes");
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



            Transform meshGO = scriptGO.GetChild(0);//Resources.Load<Transform>(resourcesLocation + "_Mesh");
            Building b = scriptGO.GetComponent<Building>();
            b.scriptPrefabLocation = resourcesLocation;
            return new KeyValuePair<Building, Transform>(b, meshGO);
        }

        /// <summary>
        /// Initializes all of the needed data for the building in question.
        /// </summary>
        /// <param name="b">The Building script of the Building Script GO.</param>
        /// <param name="t">The Mesh GameObject of the Building Mesh GO.</param>
        /// <param name="coord">The chunk coordinate where the building is placed.</param>
        /// <param name="register">Whether the Building should be initialized with Building.Init. Make false if loading buildings from save.</param>
        public void SetUpBuilding(Building b, Transform t, ChunkCoord coord, bool register = true)
        {
            //TODO: THESE HAVE BEEN COMMENTED OUT AS OF 2/13/2021, TO BE CHANGED?
            //b.transform.parent = buildingScriptParent.transform;
            //t.transform.parent = buildingMeshParent.transform;
            b.timeManager = timeManager;
            b.economyManager = economyManager;
            RegisterBuilding(coord, b, t.gameObject, register);
            b.Init(t, !register);

            if (b.mc.buildingIOManager.isConveyor)
            {
                ConveyorManager.Instance.conveyors.Add(b.GetComponent<Conveyor>());
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
