using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingSystem : MonoBehaviour
{
    [HideInInspector] public Building FocusedBuilding;
    public List<Building> RegisteredBuildings = new List<Building>();
    public LayerMask allowedFocusLayers;

    [Header("Components")]
    public TimeManager TimeManager;
    public Camera MainCamera;
    public GridManager GridManager;
    public EconomyManager EconomyManager;
    public EventSystem EventSystem;

    private bool click = false;


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

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~allowedFocusLayers))
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
