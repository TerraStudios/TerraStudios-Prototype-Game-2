//
// Developed by TerraStudios (https://github.com/TerraStudios)
//
// Copyright(c) 2020-2021 Konstantin Milev (konstantin890 | milev109@gmail.com)
// Copyright(c) 2020-2021 Yerti (UZ9)
//
// The following script has been written by either konstantin890 or Yerti (UZ9) or both.
// This file is covered by the GNU GPL v3 license. Read LICENSE.md for more information.
// Past NDA/MNDA and Confidential notices are revoked and invalid since no agreement took place. Read README.md for more information.
//

using System;
using System.Collections.Generic;
using BuildingManagement;
using DebugTools;
using ItemManagement;
using UnityEngine;
using UnityEngine.Events;

namespace BuildingModules
{
    public class BuildingIOSystem : MonoBehaviour
    {
        [Tooltip("The ModuleConnector attached to the Building")]
        public ModuleConnector mc;

        // Key is item
        // Value is quantity
        [Tooltip("A list of all the items inside of the building")]
        public Dictionary<ItemData, int> itemsInside = new Dictionary<ItemData, int>();
        //public Queue<KeyValuePair<int, ItemQueueData>> itemsToSpawn = new Queue<KeyValuePair<int, ItemQueueData>>();

        [Header("IOs")]
        [Tooltip("A list of all the BuildingIO inputs for the building")]
        public BuildingIO[] inputs;
        [Tooltip("A list of all the BuildingIO outputs for the building")]
        public BuildingIO[] outputs;

        [Header("Conveyor Properties")]
        [Tooltip("Determines whether the building is a conveyor or not. Soon to be removed in the new conveyor system.")]
        public bool isConveyor;

        [Header("Events")]
        public OnItemEnterEvent onItemEnterInput = new OnItemEnterEvent();

        #region IO Visualization

        [HideInInspector] public Vector3 buildingOffset = Vector3.zero;

        // Gizmos should only be drawn while in the editor
#if UNITY_EDITOR

        /// <summary>
        /// Renders IOs for visualization when setting up
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying && buildingOffset == Vector3.zero)
            {
                buildingOffset = new Vector3();

                Vector3 buildingSize = transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size;

                Vector3Int size = new Vector3Int(
                    Mathf.CeilToInt(Mathf.Round(buildingSize.x * 10f) / 10f),
                    Mathf.CeilToInt(Mathf.Round(buildingSize.y * 10f) / 10f),
                    Mathf.CeilToInt(Mathf.Round(buildingSize.z * 10f) / 10f));

                buildingOffset.x = size.x % 2 != 0 ? -0.5f : 0;
                buildingOffset.z = size.z % 2 != 0 ? -0.5f : 0;
            }
            else if (Application.isPlaying && !GridManager.Instance.debugMode)
                return;

            Gizmos.color = Color.red;

            // Draw inputs
            Color inputColor = new Color(0, 0.47f, 1);
            foreach (BuildingIO input in inputs) // TODO: Combine the two loops
            {
                DrawIO(input, inputColor, true);
            }

            // Draw outputs
            Color outputColor = new Color(1, 0.64f, 0);
            foreach (BuildingIO output in outputs)
            {
                DrawIO(output, outputColor);
            }

            // Reset color
            Gizmos.color = Color.white;
        }

        /// <summary>
        /// Draws a <see cref="BuildingIO"/> given a <see cref="Color"/>.
        /// </summary>
        /// <param name="io">The <see cref="BuildingIO"/> to build the visualization around</param>
        /// <param name="drawColor">The color of the voxel box and arrow</param>
        /// <param name="reversed">Determines whether the arrow should be flipped in the IO visualization</param>
        private void DrawIO(BuildingIO io, Color drawColor, bool reversed = false)
        {
            if (Application.isPlaying && io.manager == null) return;

            Vector3 direction = io.direction.GetDirection(io.manager == null ? Quaternion.identity : io.manager.mc.building.meshData.rot);

            Gizmos.color = drawColor;

            ExtDebug.DrawVoxel(GetIOPosition(io), drawColor);

            DrawBuildingArrow(GetIOPosition(io), direction, reversed);
        }

        /// <summary>
        /// Draws the visual arrow for a <see cref="BuildingIO"/> while <see cref="OnDrawGizmos"/> is running
        /// </summary>
        /// <param name="cubePosition">The position of the cube</param>
        /// <param name="direction">The direction of the input or output</param>
        /// <param name="reversed">If the bool is true, the arrow will be drawn in the position of the opposite direction but still FACING the same direction</param>
        private void DrawBuildingArrow(Vector3 cubePosition, Vector3 direction, bool reversed = false)
        {
            if (!Application.isPlaying)
                Gizmos.DrawWireCube(cubePosition, new Vector3(1f, 1f, 1f));

            // If the arrow is for the input, reverse the direction and shift it over to the opposite direction's position
            if (reversed)
            {
                cubePosition += direction * 1.5f;
                direction = -direction;
            }

            Gizmos.DrawRay(cubePosition + direction * 0.5f, direction * 0.5f);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 20.0f, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 20.0f, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(cubePosition + direction, right * 0.15f);
            Gizmos.DrawRay(cubePosition + direction, left * 0.15f);
        }
#endif
        #endregion

        #region Utilities

        /// <summary>
        /// Utility method for looping through both the inputs and outputs
        /// </summary>
        /// <param name="action">Delegate for the action taken in each IO</param>
        public void IOForEach(Action<BuildingIO> action)
        {
            foreach (BuildingIO io in inputs)
            {
                action(io);
            }

            foreach (BuildingIO io in outputs)
            {
                action(io);
            }
        }

        /// <summary>
        /// Gets and returns the first attached output to this building. 
        /// Useful for knowing what is attached to a belt.
        /// </summary>
        /// <returns>The linked input of that output.</returns>
        public BuildingIO GetFirstAttachedOutput()
        {
            foreach (BuildingIO output in outputs)
            {
                if (output.linkedIO != null)
                    return output.linkedIO;
            }

            return null;
        }

        /// <summary>
        /// Retrieves the position of the IO to connect to.
        /// </summary>
        /// <param name="io">The IO to be used</param>
        /// <returns>The position of the IO.</returns>
        public Vector3 GetIOPosition(BuildingIO io)
        {
            Vector3 meshPos = mc.building.meshData.pos;

            double yEuRot = mc.building.meshData.rot.eulerAngles.y;

            int convOffset = Application.isPlaying ? (!io.manager.mc.buildingIOManager.isConveyor ? 1 : 0) : 0; // Added if the building isn't a 1x1 structure, ignore if in scene view

            // Rotate IO around euler angles
            Vector3 ioPos = yEuRot switch
            {
                90 => -new Vector3(io.localPosition.y, 0, -io.localPosition.x + convOffset),
                180 => -new Vector3(-io.localPosition.x + convOffset, 0, -io.localPosition.y + convOffset),
                270 => -new Vector3(-io.localPosition.y + convOffset, 0, io.localPosition.x),
                _ => -new Vector3(io.localPosition.x, 0, io.localPosition.y)
            };

            ioPos += new Vector3(0.5f, 0.5f, 0.5f) + buildingOffset + meshPos;

            // Prefab position - position of IO relative to prefab - value retrieved from face of IO (see 'direction' variable above)
            return ioPos;
        }

        /// <summary>
        /// Retrieves the perpendicular adjacent IO position to the target IO
        /// </summary>
        /// <param name="io">The IO to be used</param>
        /// <returns>The target position of the IO.</returns>
        public Vector3 GetTargetIOPosition(BuildingIO io)
        {

            // If the building hasn't been initialzied, just use Quaternion.identity
            Vector3 direction = io.direction.GetDirection(io.manager == null ? Quaternion.identity : io.manager.mc.building.meshData.rot) * -1;

            return GetIOPosition(io) - direction;
        }

        /// <summary>
        /// Checks whether an item is inside the building
        /// </summary>
        /// <returns>Whether an item is inside the building</returns>
        public bool HasItemInside()
        {
            if (mc.buildingIOManager.itemsInside != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Retrieves the <see cref="BuildingIO"/> marked with <see cref="BuildingIO.isTrashcanOutput"/> 
        /// </summary>
        /// <returns>The found <see cref="BuildingIO"/>, or null if no trash output is found</returns>
        public BuildingIO GetTrashOutput()
        {
            foreach (BuildingIO output in outputs)
            {
                if (output.isTrashcanOutput)
                    return output;
            }
            return null;
        }

        public BuildingIO GetAttachedToBelt(int outputId = -1)
        {
            if (outputId == -1)
            {
                foreach (BuildingIO output in outputs)
                {
                    if (output.linkedIO != null)
                        return output.linkedIO;
                }
            }
            else
            {
                return outputs[outputId].linkedIO;
            }

            return null;
        }

        #endregion

    }

    #region IO System classes and values

    // Bellow are classes and enums necessary for the IO System.
    [Serializable]
    public class BuildingIO
    {
        // These two variables are assigned when initializing the BuildingIOManager
        [NonSerialized] [HideInInspector] public BuildingIOManager manager = null; // Needed for accessing data on the building via the IO
        [HideInInspector] public int id;

        public Vector2 localPosition;
        public IODirection direction;

        [HideInInspector] public IOType type;
        public bool isTrashcanOutput;

        public IOLinkStatus linkStatus = IOLinkStatus.Unconnected;
        [HideInInspector] public BuildingIO linkedIO; // The IO this BuildingIO is connected to, e.g. an input BuildingIO to an output
        [NonSerialized] public Transform arrow;

        public Queue<ItemQueueData> itemsToSpawn = new Queue<ItemQueueData>();

        /// <summary>
        /// Called when an <see cref="ItemData"/> attempts to enter this IO.
        /// </summary>
        /// <param name="data"></param>
        public void AttemptIOEnter(ItemData data, GameObject sceneInstance, BuildingIOManager caller)
        {
            if (!manager)
            {
                Debug.LogWarning("Can't proceed item enter. No manager for BuildingIO!");
                return;
            }

            manager.AttemptItemEnter(data, id, sceneInstance, caller);
        }
    }

    /// <summary>
    /// The enum is used to store the direction of an IO, as items can only input or output from one face of a building.
    /// </summary>
    public enum IODirection
    {
        Forward,
        Backward,
        Left,
        Right
    }

    [Serializable]
    public enum IOType
    {
        Input,
        Output
    }

    public enum IOLinkStatus
    {
        Unconnected, InvalidConnection, SuccessfulConnection
    }

    /// <summary>
    /// Properties for the event when an item attempts to 'enter' a Building.
    /// </summary>
    public class OnItemEnterEvent : UnityEvent<OnItemEnterEvent>
    {
        public BuildingIOManager caller;
        public int inputId;
        public ItemData item;
        public GameObject sceneInstance;
    }

    /// <summary>
    /// Properties needed for the item ejection system
    /// </summary>
    public class ItemQueueData
    {
        public ItemData item;
        public int outputId;
        public float timeToSpawn;
        public GameObject sceneInstance;
    }

    #endregion
}
