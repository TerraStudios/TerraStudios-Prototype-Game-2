using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridPoint 
{
    public int gridID;
    public Vector2Int point;
    public bool isOccupied;
}

[System.Serializable]
public struct Grid
{
    public Vector2Int size;
}

public class GridManager : MonoBehaviour
{
    [Header("Grid Properties")]
    public List<Grid> GridData = new List<Grid>();
    private List<GridPoint> GridPoints = new List<GridPoint>();
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
            //if (Input.GetMouseButtonDown(0))
                //Build();
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

    private void VisualizeBuild(bool forceVisualize = false)
    {
        Vector3 center = DoRay(Input.mousePosition);

        if (center == Vector3.zero && !forceVisualize)
            return;

        if (visualization == null)
        {
            visualization = Instantiate(currentBuilding.prefab, center + GetBuildingOffset(currentBuilding), RotationChange);
        }
        else if (forceVisualize)
        {
            Destroy(visualization.gameObject);
            visualization = Instantiate(currentBuilding.prefab, center + GetBuildingOffset(currentBuilding), RotationChange);
            hasRotationChanged = false;
        }
        else if (lastVisualize == center)
        {
            return;
        }
        else
        {
            Destroy(visualization.gameObject);
            visualization = Instantiate(currentBuilding.prefab, center + GetBuildingOffset(currentBuilding), RotationChange);
        }

        lastVisualize = center;
        //ShowOccupiedTiles(currentBuilding, center);
    }

    /*private void DevisualizeBuild()
    {
        if (visualization != null)
        {
            Destroy(visualization.gameObject);
        }

        DevisualizeAll();
    }*/

    /*public void Build()
    {
        Vector3 center = DoRay(Input.mousePosition);
        if (!OccupyTilesCheck(currentBuilding, center))
        {
            if (center == Vector3.zero)
                return;

            if (OccupyTiles(currentBuilding, center))
            {
                if (EconomyManager.CheckForSufficientFunds(currentBuilding.price))
                {
                    Building instantiated = Instantiate(currentBuilding, center + GetBuildingOffset(currentBuilding), RotationChange);
                    EconomyManager.Balance -= currentBuilding.price;
                    BuildingManager.SetUpBuilding(instantiated);
                    StartCoroutine(StopBuildMode());
                }
                else
                {
                    Debug.LogError("Insufficient Funds!");
                }
            }
            else
            {
                Debug.LogError("Space check requirements failed!");
            }
        }
    }*/

    IEnumerator StopBuildMode()
    {
        yield return new WaitForSeconds(.1f);
        IsInBuildMode = false;
    }

    /*private void ShowOccupiedTiles(Building b, Vector3 center)
    {
        for (int i = 0; i < visualizeOccupiedTiles.Count; i++)
        {
            visualizeOccupiedTiles[i].OnDevisualize();
        }

        List<Tile> visualizeTiles = GetTileFromSize(b, center);

        if (visualizeTiles != null)
        {
            visualizeOccupiedTiles = visualizeTiles;
            for (int i = 0; i < visualizeTiles.Count; i++)
            {
                visualizeTiles[i].OnVisualize();
            }
        }
    }*/

    /*public void DevisualizeAll()
    {
        for (int i = 0; i < visualizeOccupiedTiles.Count; i++)
        {
            visualizeOccupiedTiles[i].OnDevisualize();
        }
        visualizeOccupiedTiles.Clear();
    }*/
/*
        if (isFlipped)
        {
            //size = new Vector3(b.buildSize.y / 2f - .5f, 1, b.buildSize.x / 2f - .5f);
            size = new Vector3(b.buildSize.y, 1, b.buildSize.x);
        }
        else
        {
            //size = new Vector3(b.buildSize.x / 2f - .5f, 1, b.buildSize.y / 2f - .5f);
            size = new Vector3(b.buildSize.x, 1, b.buildSize.y);
        }
        */

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
            //DevisualizeBuild();
            for (int i = 0; i < BuildingManager.RegisteredBuildings.Count; i++)
            {
                BuildingManager.RegisteredBuildings[i].BuildingIOManager.DevisualizeAll();
            }
        }
        else
        {
            for (int i = 0; i < BuildingManager.RegisteredBuildings.Count; i++)
            {
                BuildingManager.RegisteredBuildings[i].BuildingIOManager.VisualizeAll();
            }
        }
    }
}
