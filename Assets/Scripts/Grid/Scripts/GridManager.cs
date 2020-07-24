using System.Collections;
using UnityEngine;

public struct GridLayer
{
    public bool[,] tiles;
}

public class GridManager : MonoBehaviour
{
    public static GridManager getInstance;
    //[Header("Grid Properties")]
    //public List<Grid> GridData = new List<Grid>();
    //private GridLayer[] GridLayers = new GridLayer[1];
    [Header("Components")]
    public BuildingManager BuildingManager;
    public EconomyManager EconomyManager;
    public Camera MainCamera;

    [Header("Constant variables")]
    public float tileSize;
    public LayerMask canPlaceIgnoreLayers;
       

    [Header("Dynamic variables")]
    private bool isInBuildMode;

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
    public Building building;
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

    private bool isFlipped;
    private bool click = false;

    private float lastClick = -1; //initialize as -1 to confirm first click

    public bool canPlace = false;

    #region Unity Events

    private void Awake()
    {
        getInstance = this;
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


            if (!click && (Mathf.Abs(Time.unscaledTime - lastClick) > 0.2))
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
    public void UpdateVisualization()
    {
        RaycastHit? hit = FindGridHit();
        if (hit == null) return;
        Vector3 center = GetGridPosition(hit.Value.point);

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
        TimeEngine.isPaused = true;
        visualization = Instantiate(currentBuilding.prefab, center, RotationChange).transform;
        visualization.GetComponent<Building>().SetIndicator(BuildingManager.instance.DirectionIndicator);
        tempMat = currentBuilding.prefab.GetComponent<MeshRenderer>().sharedMaterial;
    }

    /// <summary>
    /// Attempts to build the currently selected structure
    /// </summary>
    private void Build()
    {
        //Vector3 center = DoRay(Input.mousePosition);

        RaycastHit? hit = FindGridHit();
        if (hit == null) return;

        Vector3 center = GetGridPosition(hit.Value.point);

        if (center == Vector3.zero)
            return;
        if (CanPlace(hit.Value, center))
        {

            //destroyBuilding(visualization.gameObject);
            //visualization = null;
            //Transform newMachine = Instantiate(currentBuilding.prefab, center, RotationChange);

            visualization.gameObject.AddComponent<BoxCollider>();
            visualization.GetComponent<MeshRenderer>().material = tempMat;
            Building b = visualization.GetComponent<Building>();


            //b.mc.BuildingIOManager.MarkForLinking();

            BuildingManager.SetUpBuilding(b);
            b.RemoveIndicator();

            b.mc.BuildingIOManager.LinkAll();

            IsInBuildMode = Input.GetKey(KeyCode.LeftShift);

            if (visualization)
            {
                Building visBuilding = visualization.GetComponent<Building>();

                visBuilding.mc.BuildingIOManager.UpdateIOPhysics();
            }

            //b.mc.BuildingIOManager.UpdateIOPhysics(b);

            /*if (IsInBuildMode)
            {
                b.mc.BuildingIOManager.VisualizeAll();
            }*/


        }
        else
            Debug.Log("Not allowed to place here!");
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
        Vector3 location = hit.point;
        Vector2 buildingSize = currentBuilding.GetBuildSize();

        Vector3 buildingBounds = currentBuilding.GetComponent<MeshRenderer>().bounds.size;

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
    ///
    private Vector3 GetGridPosition(Vector3 pos)
    {
        Vector2Int buildSize = currentBuilding.GetBuildSize();

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
    private RaycastHit? FindGridHit()
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
            TimeEngine.isPaused = false;
            visualization = null;
            foreach (Building b in BuildingManager.RegisteredBuildings)
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
    public void OnBuildButtonPressed()
    {
        currentBuilding = building;
        IsInBuildMode = true;
    }

    /// <summary>
    /// Event for when the conveyor build button is pressed. Currently turns on IsInBuildMode and sets the current structure.
    /// </summary>
    public void OnConveyorBuildButtonPressed()
    {
        currentBuilding = conveyor;
        IsInBuildMode = true;
    }

    #endregion
}
