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
        public static readonly Dictionary<ChunkCoord, List<KeyValuePair<Building, GameObject>>> PlacedBuildings = new Dictionary<ChunkCoord, List<KeyValuePair<Building, GameObject>>>();

        private GameObject _buildingScriptParent;

        private GameObject buildingScriptParent
        {
            get
            {
                return _buildingScriptParent ? _buildingScriptParent : (_buildingScriptParent = new GameObject("Building GO Scripts"));
            }
        }

        private GameObject _buildingMeshParent;
        private GameObject buildingMeshParent
        {
            get
            {
                return _buildingMeshParent ? _buildingMeshParent : (_buildingMeshParent = new GameObject("Building GO Meshes"));
            }
        }

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
                    position = meshGO.transform.position,
                    rotation = meshGO.transform.rotation,
                    building = b.bBase,
                    scriptPrefabPath = b.prefabLocation
                };

                GameSave.current.worldSaveData.placedBuildings.Add(toSave);
            }
        }

        public static void UnRegisterBuilding(Building b)
        {
            // Remove List entry of Building b found in the KVP List in RegisteredBuildings.Values
            
            foreach (List<KeyValuePair<Building, GameObject>> kvp in PlacedBuildings.Values)
            {
                kvp.Remove(kvp.Find(kvp => kvp.Key == b));
            }

            GameSave.current.worldSaveData.placedBuildings.Where(bSave => bSave.building == b.bBase);
        }

        public void ClearRegisteredBuildings() { PlacedBuildings.Clear(); }

        /// <summary>
        /// Main update loop for the BuildingSystem, refreshes the UI with OnBuildingUpdateUI
        /// </summary>
        private void Update()
        {
            OnBuildingUpdateUI();
        }

        public void LoadAllBuildingsFromSave()
        {
            foreach (BuildingSave save in GameSave.current.worldSaveData.placedBuildings)
            {
                Transform buildingGO = Instantiate(save.GetScriptObj().gameObject, Vector3.zero, Quaternion.identity).transform;
                Transform meshGO = Instantiate(save.GetMeshObj().gameObject, save.position, save.rotation).transform;

                buildingGO.parent = buildingScriptParent.transform;
                meshGO.parent = buildingMeshParent.transform;

                Building building = buildingGO.GetComponent<Building>();
                ChunkCoord chunkCoord = save.chunkCoord;
                building.bBase = save.building;
                SetUpBuilding(building, meshGO, chunkCoord, false);
            }
        }

        /// <summary>
        /// Initializes all of the needed data for the building in question
        /// </summary>
        /// <param name="b"></param>
        public void SetUpBuilding(Building b, Transform t, ChunkCoord coord, bool register = true)
        {
            b.transform.parent = buildingScriptParent.transform;
            t.transform.parent = buildingMeshParent.transform;
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
