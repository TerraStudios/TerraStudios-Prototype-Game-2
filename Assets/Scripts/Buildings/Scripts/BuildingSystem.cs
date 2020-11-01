using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingSystem : MonoBehaviour
{
    [HideInInspector] public Building FocusedBuilding;
    public LayerMask ignoreFocusLayers;

    [Header("Components")]
    public TimeManager TimeManager;

    public Camera MainCamera;
    public GridManager GridManager;
    public EconomyManager EconomyManager;

    public static readonly List<Building> RegisteredBuildings = new List<Building>();

    private GameObject _buildingHolder;

    private GameObject buildingHolder
    {
        get
        {
            if (_buildingHolder == null)
            {
                _buildingHolder = new GameObject("Buildings");
            }

            return _buildingHolder;
        }
    }

    public static void RegisterBuilding(Building b, bool save = true)
    {
        RegisteredBuildings.Add(b);
        BuildingSave toSave = new BuildingSave()
        {
            location = b.transform.position,
            rotation = b.transform.rotation,
            building = b.Base,
            prefabLocation = b.prefabLocation
        };
        if (save)
            GameSave.current.WorldSaveData.PlacedBuildings.Add(toSave);
    }

    public static void UnRegisterBuilding(Building b)
    {
        RegisteredBuildings.Remove(b);
        GameSave.current.WorldSaveData.PlacedBuildings.Where(bSave => bSave.building == b.Base);
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
        foreach (BuildingSave b in GameSave.current.WorldSaveData.PlacedBuildings)
        {
            Transform t = Instantiate(b.GetObj().gameObject, b.location, b.rotation).transform;
            Building building = t.GetComponent<Building>();
            building.Base = b.building;
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
        b.TimeManager = TimeManager;
        b.EconomyManager = EconomyManager;
        RegisterBuilding(b, register);
        b.Init(!register);

        if (b.mc.BuildingIOManager.isConveyor)
        {
            ConveyorManager.instance.conveyors.Add(b.GetComponent<Conveyor>());
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

        Ray ray = MainCamera.ScreenPointToRay(mousePos);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.forward, 100.0F);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~ignoreFocusLayers))
        {
            Building b = hit.transform.GetComponent<Building>();
            if (b)
            {
                if (b.isSetUp)
                {
                    if (FocusedBuilding && !(b.Equals(FocusedBuilding)))
                    {
                        OnBuildingDeselected();
                    }
                    OnBuildingSelected(b);
                }
            }
            else
            {
                if (FocusedBuilding)
                {
                    OnBuildingDeselected();
                }
            }
        }
        else
        {
            if (FocusedBuilding)
            {
                OnBuildingDeselected();
            }
        }
    }

    public virtual void OnBuildingSelected(Building b) => FocusedBuilding = b;

    public virtual void OnBuildingDeselected() => FocusedBuilding = null;

    public virtual void OnBuildingUpdateUI() { }
}
