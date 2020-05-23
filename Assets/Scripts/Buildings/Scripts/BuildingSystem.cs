using System.Collections.Generic;
using UnityEngine;

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

    private void Update()
    {
        if (!GridManager.IsInBuildMode && Input.GetMouseButtonDown(0))
            CheckForHit(Input.mousePosition);
        OnBuildingUpdateUI();
    }

    public void SetUpBuilding(Building b)
    {
        b.TimeManager = TimeManager;
        b.EconomyManager = EconomyManager;
        RegisteredBuildings.Add(b);
        b.Init();
    }

    private void CheckForHit(Vector3 mousePos)
    {
        Ray ray = MainCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~allowedFocusLayers))
        {
            Building b = hit.transform.GetComponent<Building>();
            if (b)
            {
                if (b.isSetUp)
                    OnBuildingSelected(b);
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
