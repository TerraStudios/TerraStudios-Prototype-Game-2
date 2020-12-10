using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using BuildingModules;
using EconomySystem;
using TimeSystem;

namespace BuildingManagers
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

        public static readonly List<Building> RegisteredBuildings = new List<Building>();

        private GameObject _buildingHolder;

        private GameObject buildingHolder
        {
            get
            {
                return _buildingHolder ? _buildingHolder : (_buildingHolder = new GameObject("Buildings"));
            }
        }

        public static void RegisterBuilding(Building b, bool save = true)
        {
            RegisteredBuildings.Add(b);
            BuildingSave toSave = new BuildingSave()
            {
                location = b.transform.position,
                rotation = b.transform.rotation,
                building = b.bBase,
                prefabLocation = b.prefabLocation
            };
            if (save)
                GameSave.current.worldSaveData.placedBuildings.Add(toSave);
        }

        public static void UnRegisterBuilding(Building b)
        {
            RegisteredBuildings.Remove(b);
            GameSave.current.worldSaveData.placedBuildings.Where(bSave => bSave.building == b.bBase);
        }

        public void ClearRegisteredBuildings() { RegisteredBuildings.Clear(); }

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
                Transform t = Instantiate(b.GetObj().gameObject, b.location, b.rotation).transform;
                Building building = t.GetComponent<Building>();
                building.bBase = b.building;
                SetUpBuilding(building, false);
            }
        }

        /// <summary>
        /// Initializes all of the needed data for the building in question
        /// </summary>
        /// <param name="b"></param>
        public void SetUpBuilding(Building b, bool register = true)
        {
            b.transform.parent = buildingHolder.transform;
            b.timeManager = timeManager;
            b.economyManager = economyManager;
            RegisterBuilding(b, register);
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
