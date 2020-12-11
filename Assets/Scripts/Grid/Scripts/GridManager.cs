using UnityEngine;
using BuildingModules;
using EconomySystem;
using DebugTools;
using CoreManagement;

namespace BuildingManagers
{
    /// <summary>
    /// Manages the processes of building and visualizing buildings.
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance;

        [Header("Components")]
        public BuildingManager buildingManager;

        public EconomyManager economyManager;
        public Camera mainCamera;
        public GameObject removeModeEnabledText;

        [Header("Constant variables")]
        public float tileSize;

        public LayerMask canPlaceIgnoreLayers;

        [Header("Dynamic variables")]
        private bool isInBuildMode;

        [HideInInspector] public bool isInDeleteMode;
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
        public string apm1Location;
        public string apm2Location;
        public string apm3Location;
        public string conveyorLocation;

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
        private bool click;

        public bool canPlace;

        //This variable needs to be moved to a proper debugging system, only used temporarily here
        [Tooltip("Used for drawing IO collision checks in the Scene view")]
        public bool debugMode;

        #region Unity Events

        private void Awake()
        {
            Instance = this;
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
                        buildingManager.CheckForHit(Input.mousePosition);
                    }

                    click = true;
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

        #endregion Unity Events

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

        #endregion Rotation

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
                foreach (Building building in BuildingSystem.RegisteredBuildings) building.mc.buildingIOManager.VisualizeColliders();
                visualization.GetComponent<Building>().mc.buildingIOManager.VisualizeColliders();
            }

            if (center != lastVisualize || !lastRotation.Equals(RotationChange))
            {
                //Debug.Log("Found new position, updating");

                if (visualization == null)
                {
                    ConstructVisualization(center);
                }

                Building b = visualization.GetComponent<Building>();

                canPlace = CanPlace(center);

                if (canPlace)
                {
                    visualization.GetComponent<MeshRenderer>().material = buildingManager.greenArrow;
                }
                else
                {
                    visualization.GetComponent<MeshRenderer>().material = buildingManager.redArrow;
                }

                visualization.transform.position = center;
                visualization.transform.rotation = RotationChange;

                b.mc.buildingIOManager.UpdateIOPhysics();
                b.mc.buildingIOManager.UpdateArrows();
            }

            lastRotation = RotationChange;
            lastVisualize = center;
        }

        #endregion Visualization

        #region Building

        /// <summary>
        /// Instantiates the building visualization and sets the appropriate material
        /// </summary>
        /// <param name="center">Grid position for the visualization to be instantiated on</param>
        private void ConstructVisualization(Vector3 center)
        {
            buildingManager.OnBuildingDeselected();
            //TimeEngine.IsPaused = true;
            visualization = Instantiate(currentBuilding.prefab, center, RotationChange).transform;
            visualization.GetComponent<Building>().SetIndicator(BuildingManager.Instance.directionIndicator);
            tempMat = currentBuilding.prefab.GetComponent<MeshRenderer>().sharedMaterial;
        }

        /// <summary>
        /// Destroys the visualization and removes all indicators
        /// </summary>
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
            if (CanPlace(center))
            {
                Building b = visualization.GetComponent<Building>();

                if (!GameManager.Profile.allowBuildingIfBalanceInsufficient && economyManager.Balance - (decimal)b.bBase.Price < 0)
                {
                    Debug.LogWarning("Can't build because allowBuildingIfBalanceInsufficient is false");
                    return;
                }

                economyManager.Balance -= (decimal)b.bBase.Price;

                visualization.GetComponent<MeshRenderer>().material = tempMat;

                buildingManager.SetUpBuilding(b);

                IsInBuildMode = Input.GetKey(KeyCode.LeftShift);
            }
        }

        #endregion Building

        #region Grid Utilities

        /// <summary>
        /// Returns whether the currently selected building can be placed with a pivot point from a RaycastHit.
        /// </summary>
        /// <param name="grid">The grid position of the vector3 returned by the RaycastHit</param>
        /// <returns>Whether the current building can be placed at this position</returns>
        private bool CanPlace(Vector3 grid)
        {
            Vector2 buildingSize = currentBuilding.GetBuildSize();

            ExtDebug.DrawBox(grid + Vector3.up, new Vector3(buildingSize.x * 0.5f * 0.9f, 0.9f, buildingSize.y * 0.5f * 0.9f), RotationChange, Color.red);

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
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 30000f, LayerMask.GetMask("GridFloor")))
            {
                return hit;
            }
            else
            {
                return null;
            }
        }

        #endregion Grid Utilities

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
                //TimeEngine.IsPaused = false;
                visualization = null;
                if (!forceVisualizeAll)
                {
                    foreach (Building b in BuildingSystem.RegisteredBuildings)
                    {
                        if (b.mc.buildingIOManager)
                        {
                            b.mc.buildingIOManager.DevisualizeAll();
                        }
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
                    currentBuilding = GetBuildingFromLocation(apm1Location);
                    break;

                case 2:
                    currentBuilding = GetBuildingFromLocation(apm2Location);
                    break;

                case 3:
                    currentBuilding = GetBuildingFromLocation(apm3Location);
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
            currentBuilding = GetBuildingFromLocation(conveyorLocation);
            IsInBuildMode = true;
        }

        public Building GetBuildingFromLocation(string resourcesLocation)
        {
            Transform tr = Resources.Load<Transform>(resourcesLocation);
            Building b = tr.GetComponent<Building>();
            b.prefabLocation = resourcesLocation;
            return b;
        }
    }

    #endregion Events
}
