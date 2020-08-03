using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingSystem : MonoBehaviour
{
    [HideInInspector] public Building FocusedBuilding;
    [HideInInspector] public static List<Building> RegisteredBuildings = new List<Building>();
    public LayerMask ignoreFocusLayers;

    [Header("Components")]
    public TimeManager TimeManager;
    public Camera MainCamera;
    public GridManager GridManager;
    public EconomyManager EconomyManager;

    /// <summary>
    /// Main update loop for the BuildingSystem, refreshes the UI with OnBuildingUpdateUI
    /// </summary>
    private void Update()
    {
        OnBuildingUpdateUI();
    }

    /// <summary>
    /// Initializes all of the needed data for the building in question
    /// </summary>
    /// <param name="b"></param>
    public void SetUpBuilding(Building b)
    {
        b.TimeManager = TimeManager;
        b.EconomyManager = EconomyManager;
        RegisteredBuildings.Add(b);
        b.Init();

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
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = MainCamera.ScreenPointToRay(mousePos);

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
            } else
            {
                if (FocusedBuilding)
                {
                    OnBuildingDeselected();

                }
            }
        } else
        {
            if (FocusedBuilding)
            {
                OnBuildingDeselected();

            }
        }
    }

    public virtual void OnBuildingSelected(Building b)
    { 
        FocusedBuilding = b;    
    }

    public virtual void OnBuildingDeselected()
    {
        FocusedBuilding = null;
    }

    public virtual void OnBuildingUpdateUI() { }
}
