using System;
using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    [Header("Components")]
    public BuildingManager BuildingManager;
    public EconomyManager EconomyManager;
    public Camera MainCamera;
    public GameObject removeModeEnabledText;

    [Header("Constant variables")]
    public float tileSize;
    public LayerMask canPlaceIgnoreLayers;


    [Header("Dynamic variables")]
    private bool isInBuildMode;
    [HideInInspector] public bool isInDeleteMode = false;
    [HideInInspector] public bool forceVisualizeAll;

    /// <summary>
    /// Returns whether the GridManager is currently in the building mode.
    /// </summary>
    public bool IsInBuildMode
    {
        get => isInBuildMode;
        set
        {
            OnBuildModeChanged(value);
            isInBuildMode = value;
        }
    }

    private Building currentBuilding;
    public Building APM1;
    public Building APM2;
    public Building APM3;
    public Building conveyor;

    [Header("Controls / Shortcuts")]
    public KeyCode flipBuildingRight = KeyCode.R;
    public KeyCode flipBuildingLeft = KeyCode.F;

    private Vector3 lastVisualize;
    private Quaternion lastRotation;

    public Transform visualization;

    private Quaternion rotationChange = Quaternion.identity;

    private Material tempMat; //temp mat of visualization to rest to

    public Quaternion RotationChange
    {
        get => rotationChange;
        set
        {
            rotationChange = value;
            if (value == Quaternion.Euler(0, 90, 0) || value == Quaternion.Euler(0, -90, 0) || value == Quaternion.Euler(0, 270, 0) || value == Quaternion.Euler(0, -270, 0))
                isFlipped = true;
            else
                isFlipped = false;
        }
    }

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

    private bool isFlipped;
    private bool click = false;

    private float lastClick = -1; //initialize as -1 to confirm first click

    public bool canPlace = false;

    //This variable needs to be moved to a proper debugging system, only used temporarily here
    [Tooltip("Used for drawing IO collision checks in the Scene view")]
    public bool debugMode = false;


    #region Unity Events

    private void Awake()
    {
        instance = this;
    }



    /// <summary>
    /// Main update loop handles the visualization and rotation, as well as the building procedure.
    /// </summary>
    private void Update()
    {
        if (IsInBuildMode)
        {
            HandleRotation();
            UpdateVisualization();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!click)
            {
                if (IsInBuildMode)
                {
                    Build();
                }
                else
                {
                    BuildingManager.CheckForHit(Input.mousePosition);
                }

                click = true;
                lastClick = Time.unscaledTime;

            }
        }

        else if (Input.GetMouseButtonUp(0))
        {
            if (click)
            {
                click = false;
            }
        }

    }

    #endregion

    #region Rotation

    /// <summary>
    /// Checks if the input sends a rotation request to the building, applying Quaternions if necessary
    /// </summary>
    private void HandleRotation()
    {
        if (Input.GetKeyDown(flipBuildingRight))
        {
            RotationChange *= Quaternion.Euler(0, 90, 0);

        }
        if (Input.GetKeyDown(flipBuildingLeft))
        {
            RotationChange *= Quaternion.Euler(0, -90, 0);
        }
    }

    #endregion

    #region Visualization
    /// <summary>
    /// Updates the visualization position, material and IOs
    /// </summary>
    public void UpdateVisualization()
    {
        RaycastHit? hit = FindGridHit();
        if (hit == null) return;
        Vector3 center = GetGridPosition(hit.Value.point);

        //If debug mode is enabled, this will loop through every registered building as well as the visualization and call the VisualizeColliders() method
        if (debugMode && visualization)
        {
            foreach (Building building in BuildingManager.RegisteredBuildings) building.mc.BuildingIOManager.VisualizeColliders();
            visualization.GetComponent<Building>().mc.BuildingIOManager.VisualizeColliders();
        }

        if (center != lastVisualize || !lastRotation.Equals(RotationChange))
        {
            //Debug.Log("Found new position, updating");

            if (visualization == null)
            {
                ConstructVisualization(center);
            }

            Building b = visualization.GetComponent<Building>();

            canPlace = CanPlace(hit.Value, center);


            if (canPlace)
            {
                visualization.GetComponent<MeshRenderer>().material = BuildingManager.greenArrow;
            }
            else
            {
                visualization.GetComponent<MeshRenderer>().material = BuildingManager.redArrow;
            }

            visualization.transform.position = center;
            visualization.transform.rotation = RotationChange;

            b.mc.BuildingIOManager.UpdateIOPhysics();
            b.mc.BuildingIOManager.UpdateArrows();
        }


        lastRotation = RotationChange;
        lastVisualize = center;
    }

    #endregion

    #region Building

    /// <summary>
    /// Instantiates the building visualization and sets the appropriate material
    /// </summary>
    /// <param name="center">Grid position for the visualization to be instantiated on</param>
    private void ConstructVisualization(Vector3 center)
    {
        BuildingManager.OnBuildingDeselected();
        TimeEngine.IsPaused = true;
        visualization = Instantiate(currentBuilding.prefab, center, RotationChange).transform;
        visualization.GetComponent<Building>().SetIndicator(BuildingManager.instance.DirectionIndicator);
        tempMat = currentBuilding.prefab.GetComponent<MeshRenderer>().sharedMaterial;
    }

    /// <summary>
    /// Destroys the visualization and removes all indicators
    /// </summary>
    /// <param name="center">Grid position for the visualization to be instantiated on</param>
    private void DeconstructVisualization()
    {
        if (!visualization)
            return;

        visualization.GetComponent<BuildingIOManager>().DevisualizeAll();
        Destroy(visualization.gameObject);
    }

    /// <summary>
    /// Attempts to build the currently selected structure
    /// </summary>
    private void Build()
    {
        RaycastHit? hit = FindGridHit();
        if (hit == null) return;

        Vector3 center = GetGridPosition(hit.Value.point);

        if (center == Vector3.zero)
            return;
        if (CanPlace(hit.Value, center))
        {
            visualization.gameObject.AddComponent<BoxCollider>();
            visualization.GetComponent<MeshRenderer>().material = tempMat;
            Building b = visualization.GetComponent<Building>();

            BuildingManager.SetUpBuilding(b);
            b.RemoveIndicator();

            b.mc.BuildingIOManager.UpdateIOPhysics();
            b.mc.BuildingIOManager.LinkAll();



            IsInBuildMode = Input.GetKey(KeyCode.LeftShift);

            b.gameObject.transform.parent = buildingHolder.transform;

            if (visualization)
            {
                Building visBuilding = visualization.GetComponent<Building>();

                visBuilding.mc.BuildingIOManager.UpdateIOPhysics();
            }
        }

    }

    #endregion

    #region Grid Utilities

    /// <summary>
    /// Returns whether the currently selected building can be placed with a pivot point from a RaycastHit.
    /// </summary>
    /// <param name="hit">The returned RaycastHit (most likely from FindGridHit())</param>
    /// <param name="grid">The grid position of the vector3 returned by the RaycastHit</param>
    /// <returns></returns>
    private bool CanPlace(RaycastHit hit, Vector3 grid)
    {
        Vector2 buildingSize = currentBuilding.GetBuildSize();

        ExtDebug.DrawBox(grid + Vector3.up, new Vector3(buildingSize.x * 0.5f, 1f, buildingSize.y * 0.5f), RotationChange, Color.red);

        if (Physics.CheckBox(grid + Vector3.up, new Vector3(buildingSize.x * 0.5f * 0.9f, 0.9f, buildingSize.y * 0.5f * 0.9f), RotationChange, ~canPlaceIgnoreLayers))
            return false;
        else
            return true;
    }

    /// <summary>
    /// Retreives the appropriate locked grid position from a Vector3
    /// </summary>
    /// <param name="pos"></param>
    /// <returns>A locked grid position</returns>
    public Vector3 GetGridPosition(Vector3 pos, Vector2Int gridSize = default) // when argument is supplied - use it instead of GetBuildSize
    {
        Vector2Int buildSize;
        if (!Equals(gridSize, default(Vector2Int)))
            buildSize = gridSize;
        else
            buildSize = currentBuilding.GetBuildSize();

        float x;
        float z;

        if (isFlipped)
        {
            x = buildSize.y % 2 != 0 ? (Mathf.FloorToInt(pos.x) + tileSize / 2f) : Mathf.FloorToInt(pos.x);
            z = buildSize.x % 2 != 0 ? (Mathf.FloorToInt(pos.z) + tileSize / 2f) : Mathf.FloorToInt(pos.z);

        }
        else
        {
            x = buildSize.x % 2 != 0 ? (Mathf.FloorToInt(pos.x) + tileSize / 2f) : Mathf.FloorToInt(pos.x);
            z = buildSize.y % 2 != 0 ? (Mathf.FloorToInt(pos.z) + tileSize / 2f) : Mathf.FloorToInt(pos.z);
        }

        return new Vector3(x, pos.y, z);
    }

    /// <summary>
    /// Attempts to Raycast from the mouse position in order to find the grid.
    /// </summary>
    /// <returns>The RayCastHit of the floor, or null if nothing is found.</returns>
    public RaycastHit? FindGridHit()
    {
        if (Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 30000f, LayerMask.GetMask("GridFloor")))
        {
            return hit;
        }
        else
        {
            return null;
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Method called when IsInBuildMode is changed. Currently visualizes IO Ports for convenience.
    /// </summary>
    /// <param name="value">The new value for IsInBuildMode</param>
    private void OnBuildModeChanged(bool value)
    {
        RaycastHit? hit = FindGridHit();
        if (hit == null) return;

        Vector3 center = GetGridPosition(hit.Value.point);

        if (value)
        {
            ConstructVisualization(center);
        }
        else
        {
            TimeEngine.IsPaused = false;
            visualization = null;
            if (!forceVisualizeAll)
                foreach (Building b in BuildingSystem.RegisteredBuildings)
                {
                    if (b.mc.BuildingIOManager)
                    {
                        b.mc.BuildingIOManager.DevisualizeAll();
                    }
                }
        }
    }

    /// <summary>
    /// Event for when the building build button is pressed. Currently turns on IsInBuildMode and sets the current structure.
    /// </summary>
    public void OnBuildButtonPressed(int buildingID)
    {
        DeconstructVisualization();

        switch (buildingID)
        {
            case 1:
                currentBuilding = APM1;
                break;
            case 2:
                currentBuilding = APM2;
                break;
            case 3:
                currentBuilding = APM3;
                break;
        }
        
        IsInBuildMode = true;
    }

    /// <summary>
    /// Event for when the conveyor build button is pressed. Currently turns on IsInBuildMode and sets the current structure.
    /// </summary>
    public void OnConveyorBuildButtonPressed()
    {
        DeconstructVisualization();
        currentBuilding = conveyor;
        IsInBuildMode = true;
    }
}

#endregion