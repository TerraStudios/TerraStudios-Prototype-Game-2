using System.Collections;
using UnityEngine;

public struct GridLayer
{
    public bool[,] tiles;
}

public class GridManager : MonoBehaviour
{
    //[Header("Grid Properties")]
    //public List<Grid> GridData = new List<Grid>();
    //private GridLayer[] GridLayers = new GridLayer[1];
    [Header("Components")]
    public BuildingManager BuildingManager;
    public EconomyManager EconomyManager;
    public Camera MainCamera;

    [Header("Constant variables")]
    public float tileSize;
    public LayerMask buildLayer;

    [Header("Dynamic variables")]
    private bool isInBuildMode;
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
    //private List<Vector2Int> visualizeOccupiedTiles = new List<Vector2Int>();
    private Quaternion rotationChange = Quaternion.identity;

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

    private void Update()
    {
        if (IsInBuildMode)
        {
            HandleRotation();
            VisualizeBuild(hasRotationChanged);
            if (Input.GetMouseButtonDown(0))
                Build();
        }
    }

    public void OnBuildButtonPressed()
    {
        currentBuilding = building;
        IsInBuildMode = true;
    }

    public void OnConveyorBuildButtonPressed()
    {
        currentBuilding = conveyor;
        IsInBuildMode = true;
    }

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

    bool canPlace = false;
    private void VisualizeBuild(bool forceVisualize = false)
    {
        Vector3 center = DoRay(Input.mousePosition);

        if (!center.Equals(lastVisualize))
        {
            canPlace = CanPlace();
        }

        if (center == Vector3.zero && !forceVisualize)
            return;

        if (visualization == null)
        {
            visualization = Instantiate(currentBuilding.prefab, center + GetBuildingOffset(currentBuilding), RotationChange);
        }
        else if (forceVisualize)
        {
            Destroy(visualization.gameObject);
            //visualization = Instantiate(currentBuilding.prefab, center + GetBuildingOffset(currentBuilding), RotationChange);
            hasRotationChanged = false;
        }
        else if (lastVisualize == center)
        {
            return;
        }
        else
        {
            Destroy(visualization.gameObject);
            visualization.transform.position = center + GetBuildingOffset(currentBuilding);
            visualization.transform.rotation = RotationChange;
            //visualization = Instantiate(currentBuilding.prefab, center + GetBuildingOffset(currentBuilding), RotationChange);
        }

        lastVisualize = center;

        if (canPlace)
            visualization.GetComponent<MeshRenderer>().material.color = Color.green;
        else
            visualization.GetComponent<MeshRenderer>().material.color = Color.red;
    }

    private void Build()
    {
        Vector3 center = DoRay(Input.mousePosition);
        if (center == Vector3.zero)
            return;
        if (CanPlace())
        {
            IsInBuildMode = false;
            Transform newMachine = Instantiate(currentBuilding.prefab, center + GetBuildingOffset(currentBuilding), RotationChange);
            newMachine.gameObject.AddComponent<BoxCollider>();
            Building b = newMachine.GetComponent<Building>();
            BuildingManager.SetUpBuilding(b);
        }
        else
            Debug.Log("Not allowed to place here!");
    }

    //private int gridSize = 1;
    //private Vector3 gride;

    private bool CanPlace()
    {
        if (Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 30000f, LayerMask.GetMask("GridFloor")))
        {

            Vector3 location = hit.point;

            //Vector3 size = hit.collider.bounds.size;

            Vector3 grid = GetGridPosition(location);
            //gride = grid;

            Vector2 buildingSize = currentBuilding.buildSize;


            ExtDebug.DrawBox(grid + GetBuildingOffset(currentBuilding) - new Vector3(0, GetBuildingOffset(currentBuilding).y, 0)  + new Vector3(0, 0.5f, 0), new Vector3(buildingSize.x * 0.5f * 0.9f, 0.9f, buildingSize.y * 0.5f * 0.9f), RotationChange * Quaternion.Euler(0, -90, 0), Color.red);

            //Debug.Log("Grid Slot: " + grid);

            LayerMask colliderMask = ~(1 << LayerMask.NameToLayer("IOPort"));

            

            if (Physics.CheckBox(grid + GetBuildingOffset(currentBuilding) - new Vector3(0, GetBuildingOffset(currentBuilding).y, 0) + new Vector3(0, 0.5f, 0), new Vector3(buildingSize.x * 0.5f * 0.9f, 0.9f, buildingSize.y * 0.5f * 0.9f), RotationChange * Quaternion.Euler(0, -90, 0), colliderMask))
                return false;
            else
                return true;
        }
        return false;
    }

    IEnumerator StopBuildMode()
    {
        yield return new WaitForSeconds(.1f);
        IsInBuildMode = false;
    }

    public Vector3 DoRay(Vector3 mousePos)
    {
        RaycastHit[] hits = Physics.RaycastAll(MainCamera.ScreenPointToRay(mousePos));

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.gameObject.layer == 8)
                return GetGridPosition(hits[i].point);
        }

        return Vector3.zero;
    }

    private Vector3 GetGridPosition(Vector3 pos)
    {
        if (currentBuilding.hasCentricTile)
            return new Vector3(
                Mathf.FloorToInt(pos.x) + tileSize / 2,
                Mathf.FloorToInt(pos.y) + .5f,
                Mathf.FloorToInt(pos.z) + tileSize / 2);
        else
            return new Vector3(
                Mathf.FloorToInt(pos.x),
                Mathf.FloorToInt(pos.y + .5f) + .5f,
                Mathf.FloorToInt(pos.z));
    }

    private Vector3 GetBuildingOffset(Building b)
    {
        if (isFlipped)
        {
            return new Vector3(b.offsetFromCenter.z, b.offsetFromCenter.y, b.offsetFromCenter.x);
        }
        else
        {
            return b.offsetFromCenter;
        }
    }

    private void OnBuildModeChanged(bool value)
    {
        if (!value)
        {
            foreach(Building b in BuildingManager.RegisteredBuildings)
            {
                if (b.BuildingIOManager != null)
                    b.BuildingIOManager.DevisualizeAll();
            }
        }
        else
        {
            foreach (Building b in BuildingManager.RegisteredBuildings)
            {
                if (b.BuildingIOManager != null)
                    b.BuildingIOManager.VisualizeAll();
            }
        }
    }
}
