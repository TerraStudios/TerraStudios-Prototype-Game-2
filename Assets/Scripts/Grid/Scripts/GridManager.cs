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
    private Transform visualization;

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
            hasRotationChanged = true;
        }
    }

    private bool hasRotationChanged;
    private bool isFlipped;
    private bool click = false;

    private float lastClick = -1; //initialize as -1 to confirm first click

    private bool canPlace = false;

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
            VisualizeBuild(hasRotationChanged);
        }

        if (Input.GetMouseButtonDown(0))
        {
            

            if (!click && (Mathf.Abs(Time.time - lastClick) > 0.2))
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
                lastClick = Time.time;

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

    #region Visualization

    /// <summary>
    /// Attempts to visualize the currently selected structure, showing as green or red depending on the return of CanPlace
    /// </summary>
    /// <param name="forceVisualize">Forces the building to be visualized, used when rotating</param>
    private void VisualizeBuild(bool forceVisualize = false)

    {

        RaycastHit? hit = FindGridHit();
        if (hit == null) return;
        Vector3 center = GetGridPosition(hit.Value.point);

        if (!center.Equals(lastVisualize))
        {
            canPlace = CanPlace(hit.Value, center);
        }

        if (center == Vector3.zero && !forceVisualize)
            return;

        if (visualization == null)
        {

            visualization = Instantiate(currentBuilding.prefab, center, RotationChange);// + GetBuildingOffset(currentBuilding), RotationChange);
            tempMat = visualization.GetComponent<MeshRenderer>().material;
        }
        else if (forceVisualize)
        {
            visualization.transform.rotation = rotationChange;
            visualization.transform.position = center; //+ GetBuildingOffset(currentBuilding);
            hasRotationChanged = false;
        }
        else if (lastVisualize == center)
        {
            return;
        }
        else
        {
            //Destroy(visualization.gameObject);
            visualization.transform.position = center;
            visualization.transform.rotation = RotationChange;
        }

        lastVisualize = center;

        if (canPlace)
            visualization.GetComponent<MeshRenderer>().material = BuildingManager.greenArrow;
        else
            visualization.GetComponent<MeshRenderer>().material = BuildingManager.redArrow;

        visualization.GetComponent<Building>().SetIndicator(BuildingManager.instance.BuildingDirectionPrefab);
    }

    #endregion

    #region Building

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
            

            b.mc.BuildingIOManager.MarkForLinking();

            BuildingManager.SetUpBuilding(b);
            b.RemoveIndicator();

            b.mc.BuildingIOManager.LinkAll();
            

            if (Input.GetKey(KeyCode.LeftShift))
            {
                visualization = Instantiate(currentBuilding.prefab, center, RotationChange).transform;
                tempMat = visualization.GetComponent<MeshRenderer>().material;
            }
            else
            {
                visualization = null;
            }

            IsInBuildMode = Input.GetKey(KeyCode.LeftShift);


        }
        else
            Debug.Log("Not allowed to place here!");
    }

    IEnumerator destroyBuilding(GameObject obj)
    {
        yield return new WaitForEndOfFrame();
        Destroy(obj);
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
        if (!value)
        {
            foreach (Building b in BuildingManager.RegisteredBuildings)
            {
                if (b.mc.BuildingIOManager != null)
                    b.mc.BuildingIOManager.DevisualizeAll();
            }
        }
        else
        {
            foreach (Building b in BuildingManager.RegisteredBuildings)
            {
                if (b.mc.BuildingIOManager != null)
                    b.mc.BuildingIOManager.VisualizeAll();
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
