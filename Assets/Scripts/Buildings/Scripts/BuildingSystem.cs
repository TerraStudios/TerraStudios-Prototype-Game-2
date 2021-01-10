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
        /// Key - Coordinates of the chunk where the Building is placed
        /// Value - KeyValuePair containing the Building (Script Prefab) as a key and GameObject (Mesh Prefab) as a value
        /// </summary>
        public static readonly Dictionary<ChunkCoord, KeyValuePair<Building, GameObject>> PlacedBuildings = new Dictionary<ChunkCoord, KeyValuePair<Building, GameObject>>();

        private GameObject _buildingHolder;

        private GameObject buildingHolder
        {
            get
            {
                return _buildingHolder ? _buildingHolder : (_buildingHolder = new GameObject("Buildings"));
            }
        }

        public static void RegisterBuilding(ChunkCoord chunkCoord, Building b, GameObject go, bool save = true)
        {
            PlacedBuildings.Add(chunkCoord, new KeyValuePair<Building, GameObject>(b, go));
            if (save)
            {
                BuildingSave toSave = new BuildingSave()
                {
                    chunkCoord = chunkCoord,
                    rotation = b.transform.rotation,
                    building = b.bBase,
                    prefabLocation = b.prefabLocation
                };

                GameSave.current.worldSaveData.placedBuildings.Add(toSave);
            }
        }

        public static void UnRegisterBuilding(Building b)
        {
            // Remove Dict entry of Building b found in the KVP in RegisteredBuildings.Values
            PlacedBuildings.Remove(PlacedBuildings.FirstOrDefault(kvp => kvp.Value.Key == b).Key);
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
            foreach (BuildingSave b in GameSave.current.worldSaveData.placedBuildings)
            {
                Transform t = Instantiate(b.GetObj().gameObject, b.chunkCoord.ToWorldSpace(), b.rotation).transform;
                Building building = t.GetComponent<Building>();
                ChunkCoord chunkCoord = b.chunkCoord;
                building.bBase = b.building;
                SetUpBuilding(building, t, chunkCoord, false);
            }
        }

        /// <summary>
        /// Initializes all of the needed data for the building in question
        /// </summary>
        /// <param name="b"></param>
        public void SetUpBuilding(Building b, Transform t, ChunkCoord coord, bool register = true)
        {
            b.transform.parent = buildingHolder.transform;
            b.timeManager = timeManager;
            b.economyManager = economyManager;
            RegisterBuilding(coord, b, t.gameObject, register);
            b.Init(!register);

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
