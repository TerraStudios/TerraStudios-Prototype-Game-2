//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using BuildingModules;
using CoreManagement;
using DebugTools;
using EconomyManagement;
using TerrainGeneration;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.Mathematics;
using Utilities;
using TerrainTypes;
using System.Collections;

namespace BuildingManagement
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

        private KeyValuePair<Building, Transform> currentBuilding;

        private Vector3 lastVisualize;
        private Quaternion lastRotation;

        public KeyValuePair<Building, Transform> visualization;

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
        public bool canPlace;

        //This variable needs to be moved to a proper debugging system, only used temporarily here
        [Tooltip("Used for drawing IO collision checks in the Scene view")]
        public bool debugMode;

        private Vector2 mousePos;
        private float rotationDirection;
        private bool continueBuilding;

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
                UpdateVisualization();
        }

        #endregion

        #region Rotation

        public void RotateBuildingLeft(InputAction.CallbackContext context)
        {
            if (context.performed)
                RotationChange *= Quaternion.Euler(0, -90, 0);
        }

        public void RotateBuildingRight(InputAction.CallbackContext context)
        {
            if (context.performed)
                RotationChange *= Quaternion.Euler(0, 90, 0);
        }

        #endregion

        #region Visualization

        public void MousePosition(InputAction.CallbackContext context) => mousePos = context.ReadValue<Vector2>();

        /// <summary>
        /// Updates the visualization position, material and IOs
        /// </summary>
        public void UpdateVisualization()
        {
            RaycastHit? hit = FindGridHit();
            if (hit == null) return;
            Vector3 center = GetGridPosition(hit.Value.point);

            //If debug mode is enabled, this will loop through every registered building as well as the visualization and call the VisualizeColliders() method
            if (debugMode && visualization.Key)
            {
                foreach (List<KeyValuePair<Building, GameObject>> kvp in BuildingSystem.PlacedBuildings.Values)
                    foreach (KeyValuePair<Building, GameObject> buildingKVP in kvp)
                        buildingKVP.Key.mc.buildingIOManager.VisualizeColliders();

                visualization.Key.mc.buildingIOManager.VisualizeColliders();
            }

            if (center != lastVisualize || !lastRotation.Equals(RotationChange))
            {
                //Debug.Log("Found new position, updating");

                if (!visualization.Value)
                {
                    ConstructVisualization(center);
                }

                canPlace = CanPlace(center);

                if (canPlace)
                {
                    visualization.Value.GetComponent<MeshRenderer>().material = buildingManager.greenArrow;
                }
                else
                {
                    visualization.Value.GetComponent<MeshRenderer>().material = buildingManager.redArrow;
                }

                visualization.Value.transform.position = center;
                visualization.Value.transform.rotation = RotationChange;
                visualization.Key.mc.buildingIOManager.UpdateArrows();
            }

            lastRotation = RotationChange;
            lastVisualize = center;
        }

        #endregion

        #region Building

        public void BuildTrigger(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (IsInBuildMode)
                {
                    Build();
                }
                else
                {
                    buildingManager.CheckForHit(mousePos);
                }
            }
        }

        public void ContinueBuildingTrigger(InputAction.CallbackContext context) => continueBuilding = !continueBuilding;

        /// <summary>
        /// Instantiates the building visualization and sets the appropriate material
        /// </summary>
        /// <param name="center">Grid position for the visualization to be instantiated on</param>
        private void ConstructVisualization(Vector3 center)
        {
            buildingManager.OnBuildingDeselected();
            //TimeEngine.IsPaused = true;

            visualization = new KeyValuePair<Building, Transform>(
                 Instantiate(currentBuilding.Key.prefab, Vector3.zero, RotationChange).GetComponent<Building>(),
                 ObjectPoolManager.Instance.ReuseObject(currentBuilding.Value.gameObject, center, RotationChange).transform
                );

            visualization.Key.correspondingMesh = visualization.Value;

            if (visualization.Key.transform.childCount != 0)
                Destroy(visualization.Key.transform.GetChild(0).gameObject);
            else
                Debug.LogWarning("Building script GO doesn't have a mesh!");

            visualization.Key.SetIndicator(BuildingManager.Instance.directionIndicator);
            tempMat = currentBuilding.Value.GetComponent<MeshRenderer>().sharedMaterial;
        }

        /// <summary>
        /// Destroys the visualization and removes all indicators
        /// </summary>
        private void DeconstructVisualization()
        {
            if (!visualization.Value)
                return;

            visualization.Key.GetComponent<BuildingIOManager>().DevisualizeAll();
            Destroy(visualization.Value.gameObject);
        }

        /// <summary>
        /// Attempts to build the currently selected structure
        /// </summary>
        private void Build()
        {
            RaycastHit? hit = FindGridHit();
            if (hit == null) return;

            Vector3 center = GetGridPosition(hit.Value.point);

            ChunkCoord chunkCoord = new ChunkCoord { x = Mathf.FloorToInt(center.x) / TerrainGenerator.Instance.chunkXSize, z = Mathf.FloorToInt(center.z) / TerrainGenerator.Instance.chunkZSize }; //! Figure out this somehow

            if (center == Vector3.zero)
                return;
            if (CanPlace(center))
            {
                if (!GameManager.Instance.CurrentGameProfile.allowBuildingIfBalanceInsufficient && economyManager.Balance - (decimal)visualization.Key.bBase.Price < 0)
                {
                    Debug.LogWarning("Can't build because allowBuildingIfBalanceInsufficient is false");
                    return;
                }

                economyManager.Balance -= (decimal)visualization.Key.bBase.Price;

                visualization.Value.GetComponent<MeshRenderer>().material = tempMat;

                TerrainGenerator generator = TerrainGenerator.Instance;

                // Set position in the chunk it was placed in

                int3 voxelPos = visualization.Value.position.FloorToInt3();
                Vector3Int buildingSize = GetBuildSize(currentBuilding.Value);

                Chunk placedChunk = generator.currentChunks[chunkCoord];

                // TODO: Move this to an enum
                VoxelType type = new VoxelType { isSolid = true };
                MachineSlaveVoxel slaveBlock = new MachineSlaveVoxel(type, visualization.Key);
                //int3 localVoxelPos = generator.GetRelativeChunkPosition(voxelPos.x, voxelPos.y, voxelPos.z);

                //placedChunk.SetVoxelData(localVoxelPos.x, localVoxelPos.y, localVoxelPos.z, slaveBlock);

                for (int x = voxelPos.x - buildingSize.x + 1; x <= voxelPos.x; x++)
                {
                    for (int y = voxelPos.y; y < voxelPos.y + buildingSize.y; y++)
                    {
                        for (int z = voxelPos.z - buildingSize.z + 1; z <= voxelPos.z; z++)
                        {
                            if (!placedChunk.VoxelInsideChunk(x, y, z))
                            {
                                int3 localPos = generator.GetRelativeChunkPosition(x, y, z);
                                generator.currentChunks[generator.GetChunkCoord(x, y, z)].SetVoxelData(localPos.x, localPos.y, localPos.z, slaveBlock);
                            }
                            else
                            {
                                placedChunk.SetVoxelData(x, y, z, slaveBlock);
                            }

                        }
                    }
                }

                visualization.Key.meshData = new MeshData()
                {
                    pos = visualization.Value.position,
                    rot = visualization.Value.rotation,
                    size = buildingSize
                };

                RegisterBuildingData data = new RegisterBuildingData()
                {
                    building = visualization.Key,
                    buildingMesh = visualization.Value,
                    buildingMeshPrefab = currentBuilding.Value,
                    chunkCoord = chunkCoord
                };

                buildingManager.SetUpBuilding(data);

                IsInBuildMode = continueBuilding;
            }
        }

        #endregion

        #region Grid Utilities

        // TODO: This code was for showing visualizations of the collision system during the development and is no longer needed. However, if we want to move this to a debugging option this code
        // TODO: will remain commented for later use.
        /*

        private Vector3 currentGrid;

        private IEnumerator testGridCheck()
        {

            while (true)
            {
                Vector3 grid = currentGrid;

                Vector3Int buildingSize = GetBuildSize(currentBuilding.Value);

                for (int x = (int)grid.x - buildingSize.x + 1; x <= grid.x; x++)
                {
                    for (int y = (int)grid.y; y < grid.y + buildingSize.y; y++)
                    {
                        for (int z = (int)grid.z - buildingSize.z + 1; z <= grid.z; z++)
                        {
                            ExtDebug.DrawBox(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), Vector3.one / 2f, Quaternion.identity, Color.cyan);
                        }
                    }
                }

                ExtDebug.DrawBox(grid + Vector3.one / 2f, Vector3.one / 2f, Quaternion.identity, Color.red);

                yield return new WaitForEndOfFrame();
            }
            //testGridCheck(grid);
        }

        */

        /// <summary>
        /// Returns whether the currently selected building can be placed with a pivot point from a RaycastHit.
        /// </summary>
        /// <param name="grid">The grid position of the vector3 returned by the RaycastHit</param>
        /// <returns>Whether the current building can be placed at this position</returns>
        private bool CanPlace(Vector3 grid)
        {
            // Part of above TODO statement
            //currentGrid = grid;
            //StartCoroutine(testGridCheck());

            Vector3Int buildingSize = GetBuildSize(currentBuilding.Value);

            for (int x = (int)grid.x - buildingSize.x + 1; x <= grid.x; x++)
            {
                for (int y = (int)grid.y; y < grid.y + buildingSize.y; y++)
                {
                    for (int z = (int)grid.z - buildingSize.z + 1; z <= grid.z; z++)
                    {
                        // Check if the floor below IS all solid
                        if (y == (int)grid.y)
                        {
                            if (!TerrainGenerator.Instance.voxelTypes[TerrainGenerator.Instance.GetVoxelValue(new int3(x, y - 1, z))].isSolid)
                            {
                                return false;
                            }
                        }

                        // Check if the entire space the building will occupy IS NOT solid
                        if (TerrainGenerator.Instance.voxelTypes[TerrainGenerator.Instance.GetVoxelValue(new int3(x, y, z))].isSolid)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Retreives the appropriate locked grid position from a Vector3
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>A locked grid position</returns>
        public Vector3 GetGridPosition(Vector3 pos, Vector3Int gridSize = default) // when argument is supplied - use it instead of GetBuildSize
        {
            Vector3Int buildSize;
            if (!Equals(gridSize, default(Vector3Int)))
                buildSize = gridSize;
            else
                buildSize = GetBuildSize(currentBuilding.Value);

            float x;
            float z;

            if (isFlipped)
            {
                x = buildSize.z % 2 != 0 ? (Mathf.FloorToInt(pos.x) + tileSize / 2f) : Mathf.FloorToInt(pos.x);
                z = buildSize.x % 2 != 0 ? (Mathf.FloorToInt(pos.z) + tileSize / 2f) : Mathf.FloorToInt(pos.z);
            }
            else
            {
                x = buildSize.x % 2 != 0 ? (Mathf.FloorToInt(pos.x) + tileSize / 2f) : Mathf.FloorToInt(pos.x);
                z = buildSize.z % 2 != 0 ? (Mathf.FloorToInt(pos.z) + tileSize / 2f) : Mathf.FloorToInt(pos.z);
            }

            return new Vector3(x, Mathf.FloorToInt(pos.y + 0.1f), z);
            //ExtDebug.DrawBox(new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.RoundToInt(pos.z)), new Vector3(0.5f, 0.5f, 0.5f), RotationChange, Color.white);
            //return new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y + 0.1f), Mathf.RoundToInt(pos.z));
        }

        /// <summary>
        /// Attempts to Raycast from the mouse position in order to find the grid.
        /// </summary>
        /// <returns>The RayCastHit of the floor, or null if nothing is found.</returns>
        public RaycastHit? FindGridHit()
        {
            if (Physics.Raycast(mainCamera.ScreenPointToRay(mousePos), out RaycastHit hit, 300000f, ~canPlaceIgnoreLayers))
            {
                return hit;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves the grid size of the building
        /// </summary>
        /// <returns>A <see cref="Vector2Int"/> representing the grid size</returns>
        public Vector3Int GetBuildSize(Transform mesh)
        {
            Vector3 e = mesh.GetComponent<MeshRenderer>().bounds.size;
            return new Vector3Int(Mathf.CeilToInt(Mathf.Round(e.x * 10f) / 10f), Mathf.CeilToInt(Mathf.Round(e.y * 10f) / 10f), Mathf.CeilToInt(Mathf.Round(e.z * 10f) / 10f));
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
                //TimeEngine.IsPaused = false;
                visualization = new KeyValuePair<Building, Transform>(null, null);
                if (!forceVisualizeAll)
                {
                    foreach (List<KeyValuePair<Building, GameObject>> kvp in BuildingSystem.PlacedBuildings.Values)
                        foreach (KeyValuePair<Building, GameObject> buildingKVP in kvp)
                        {
                            if (buildingKVP.Key.mc.buildingIOManager)
                            {
                                buildingKVP.Key.mc.buildingIOManager.DevisualizeAll();
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

            int resourceID = buildingID - 1;

            currentBuilding = BuildingManager.Instance.GetBuildingFromLocation(resourceID);
            currentBuilding.Key.correspondingMesh = currentBuilding.Value;

            IsInBuildMode = true;
        }

        /// <summary>
        /// Event for when the conveyor build button is pressed. Currently turns on IsInBuildMode and sets the current structure.
        /// </summary>
        public void OnConveyorBuildButtonPressed()
        {
            DeconstructVisualization();
            currentBuilding = BuildingManager.Instance.GetBuildingFromLocation(3);
            IsInBuildMode = true;
        }


    }

    #endregion
}
