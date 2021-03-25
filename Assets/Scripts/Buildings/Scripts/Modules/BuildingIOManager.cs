//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using DebugTools;
using ItemManagement;
using System;
using System.Collections.Generic;
using TerrainGeneration;
using TerrainTypes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace BuildingModules
{
    [Serializable]
    public class BuildingIO
    {
        public Vector2 localPosition;
        public IODirection direction;

        //TODO: Remove this?
        private IOType type;
        public bool isTrashcanOutput;

        [HideInInspector]
        public BuildingIO linkedIO; // The IO this BuildingIO is connected to, e.g. an input BuildingIO to an output
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

    /// <summary>
    /// Because C# is bad, an extension method is needed to add a method for each direction. The extension retrieves the <see cref="Vector3Int"/> value for code later on.
    /// </summary>
    /// Usage: <code>IODirection.GetDirection();</code>
    public static class IODirectionExtension
    {
        /// <summary>
        /// Fetches the <see cref="Vector3Int"/> value of an <see cref="IODirection"/>
        /// </summary>
        /// <param name="direction">The <see cref="IODirection"/></param>
        /// <returns>A normalized <see cref="Vector3Int"/> for manipulating the direction</returns>
        public static Vector3Int GetDirection(this IODirection direction)
        {
            switch (direction)
            {
                case IODirection.Forward:
                    return new Vector3Int(1, 0, 0);
                case IODirection.Backward:
                    return new Vector3Int(-1, 0, 0);
                case IODirection.Left:
                    return new Vector3Int(0, 0, 1);
                case IODirection.Right:
                    return new Vector3Int(0, 0, -1);
                default:
                    return Vector3Int.zero;
            }
        }
    }

    [Serializable]
    public enum IOType
    {
        Input,
        Output
    }

    /// <summary>
    /// Properties for the event when an item attempts to 'enter' a Building.
    /// </summary>
    public class OnItemEnterEvent : UnityEvent<OnItemEnterEvent>
    {
        public int inputID;
        public ItemData item;
        public GameObject sceneInstance;
        public Dictionary<ItemData, int> proposedItems;
    }

    /// <summary>
    /// Properties of an item that is present 'inside' the Building.
    /// </summary>
    public class ItemInsideData
    {
        public int quantity;
        public ItemData item;
    }

    /// <summary>
    /// Manages the Building IO system.
    /// </summary>
    public class BuildingIOManager : MonoBehaviour
    {
        [Tooltip("The ModuleConnector attached to the Building")]
        public ModuleConnector mc;

        // Key is item
        // Value is quantity
        [Tooltip("A list of all the items inside of the building")]
        public Dictionary<ItemData, int> itemsInside = new Dictionary<ItemData, int>();

        [Header("IOs")]
        [Tooltip("A list of all the BuildingIO inputs for the building")]
        public BuildingIO[] inputs;
        [Tooltip("A list of all the BuildingIO outputs for the building")]
        public BuildingIO[] outputs;

        [Header("Conveyor Properties")]
        [Tooltip("Determines whether the building is a conveyor or not. Soon to be removed in the new conveyor system.")]
        public bool isConveyor;

        [Header("Events")]
        public OnItemEnterEvent OnItemEnterInput;

        /// <summary>
        /// Initializes all of the <see cref="Building">'s <see cref="BuildingIO"/>s and calls the <see cref="OnItemEnterEvent"/> event. 
        /// </summary>
        public void Init()
        {
            /*IOForEach(io => io.Init());

            for (int i = 0; i < inputs.Length; i++)
                inputs[i].ID = i;

            for (int i = 0; i < outputs.Length; i++)
                outputs[i].ID = i;
            */

            OnItemEnterInput = new OnItemEnterEvent();
        }

        /// <summary>
        /// Updates the position of the arrow <see cref="GameObject"/> to the current IO's position. 
        /// Currently used for updating the position of any arrows on the visualization (which can be moved).
        /// </summary>
        public void UpdateArrows()
        {
            // TODO: Update with new code here

            /*IOForEach(io =>
            {
                if (io.arrow)
                {
                    io.arrow.position = io.transform.position + new Vector3(0, 1, 0);
                    io.arrow.rotation = io.transform.rotation;
                }
            });*/
        }

        /// <summary>
        /// Updates all of the Physics related information for a visualization (<see cref="Physics.OverlapBox"/> being one of them)
        /// </summary>
        public void UpdateIOPhysics()
        {
            //IOForEach(io => io.OnVisualizationMoved());
        }

        private int3 GetIOPosition(BuildingIO io)
        {
            Vector3 prefabPosition = mc.building.meshData.pos;
            Vector3 direction = io.direction.GetDirection() * -1;

            // Prefab position - position of IO relative to prefab - value retrieved from face of IO (see 'direction' variable above)
            //TODO: Move localPosition to Vector2Int to avoid casting
            return prefabPosition.FloorToInt3() - new int3((int)io.localPosition.x, 0, (int)io.localPosition.y) - direction.FloorToInt3();


        }

        private void AttemptLink(BuildingIO io, bool input = true)
        {
            // Retrieves the OPPOSITE voxel (normal vector with a magnitude of 1 voxel)
            int3 linkVoxelPos = GetIOPosition(io);
            Voxel targetvoxel = TerrainGenerator.instance.GetVoxel(linkVoxelPos);

            if (targetvoxel is MachineSlaveVoxel voxel)
            {
                BuildingIOManager targetBuilding = voxel.controller.mc.buildingIOManager;

                // Loop through opposite of io's type
                foreach (BuildingIO targetIO in input ? targetBuilding.outputs : targetBuilding.inputs)
                {
                    // Get the position of the voxel perpendicular to the target IO, and check if it equals the desired linkVoxelPos
                    if ((targetBuilding.mc.building.meshData.pos.FloorToInt3() - new int3((int)targetIO.localPosition.x, 0, (int)targetIO.localPosition.y)).Equals(linkVoxelPos))
                    {
                        // Found successful link, set linkedIO for both
                        targetIO.linkedIO = io;
                        io.linkedIO = targetIO;

                        Debug.Log("Successfully linked");
                    }
                }
            }

        }

        /// <summary>
        /// Signals every <see cref="BuildingIO"/> in the building to attempt a link with any other attached <see cref="BuildingIO"/>s.
        /// </summary>
        public void LinkAll()
        {
            foreach (BuildingIO output in outputs)
            {
                AttemptLink(output, false);
            }

            foreach (BuildingIO input in inputs)
            {
                AttemptLink(input);
            }
        }

        /// <summary>
        /// Signals every <see cref="BuildingIO"/> in the building to attempt to unlink with any other attached <see cref="BuildingIO"/>s.
        /// 
        /// If the <see cref="BuildingIO"/> has no link, no operation will be executed.
        /// </summary>
        public void UnlinkAll()
        {
            IOForEach(io =>
            {
                if (io.linkedIO != null)
                {
                    io.linkedIO.linkedIO = null;
                    io.linkedIO = null;
                }

            });
            // TODO: Update with new code here
            //IOForEach(io => io.Unlink());
        }

        public void ProceedItemEnter(GameObject sceneInstance, ItemData item, int inputID)
        {
            Dictionary<ItemData, int> proposed = new Dictionary<ItemData, int>(itemsInside);

            if (proposed.ContainsKey(item))
            {
                proposed[item]++;
            }
            else
            {
                proposed[item] = 1;
            }

            OnItemEnterEvent args = new OnItemEnterEvent()
            {
                inputID = inputID,
                item = item,
                sceneInstance = sceneInstance,
                proposedItems = proposed
            };
            OnItemEnterInput.Invoke(args);

            Debug.Log("Item fully in me! Item is " + item.name);
        }

        public void AcceptItemEnter(OnItemEnterEvent args)
        {

        }

        public void TrashItem(GameObject sceneInstance, ItemData item)
        {
            Destroy(sceneInstance, 1f);
            BuildingIO trashOutput = GetTrashOutput();

            // TODO: Update with new code here. Instantiate item in the output

            //trashOutput.AddToSpawnQueue(item);
        }

        /// <summary>
        /// Signals every <see cref="BuildingIO"> in the building to create a blue arrow <see cref="GameObject"/> visualization above it.
        /// </summary>
        public void VisualizeAll()
        {
            // TODO: Update with new code here
            //IOForEach(io => io.VisualizeArrow());
        }

        /// <summary>
        /// Signals every <see cref="BuildingIO"> in the building to stop visualizing.
        /// </summary>
        public void DevisualizeAll()
        {
            // TODO: Update with new code here
            //IOForEach(io => io.Devisualize());
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

        /// <summary>
        /// Modifies the conveyor group belonging to a <see cref="Building"/> and sets each one to a given state
        /// </summary>
        /// <param name="state">The new <see cref="WorkStateEnum"/> for the conveyor group to be in</param>
        public void SetConveyorGroupState(WorkStateEnum state)
        {
            foreach (BuildingIOManager bIO in GetConveyorGroup())
            {
                bIO.mc.building.SetWorkstateSilent(state); //set it silently to not trigger on workstate changed (recursion)
            }
        }

        /// <summary>
        /// Retrieves the conveyor group of any building ID.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of <see cref="BuildingIOManager"/>s</returns>
        private List<BuildingIOManager> GetConveyorGroup()
        {
            List<BuildingIOManager> toReturn = new List<BuildingIOManager>();
            RecursiveGetConveyorGroup(toReturn, true);
            //Debug.Log(isConveyor);
            if (isConveyor) RecursiveGetConveyorGroup(toReturn, false);

            //Log.LogConsole($"Found {toReturn.Count} conveyors");

            return toReturn;
        }


        /// <summary>
        /// Recursive method for retrieving the conveyor group of any Building ID. Works by adding all of the attached IOs of a building and its attached IOs as well (until a non conveyor is found or there isn't another attached IO)
        /// </summary>
        /// <param name="currentList">A list of <see cref="BuildingIOManager"/>s that will end up being the result. Passed through as a pointer reference.</param>
        /// <param name="getInputs">Determines whether it should also search through the building's inputs as well</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="BuildingIOManager"></see> in the <paramref name="getInputs"/> parameter</returns>
        private void RecursiveGetConveyorGroup(List<BuildingIOManager> currentList, bool getInputs = true)
        {
            // TODO: Update with new code here to find the chain of belts attached

            /*foreach (BuildingIO io in getInputs ? inputs : outputs)
            {
                if (io.attachedIO && io.attachedIO.ioManager.isConveyor && !currentList.Contains(io.attachedIO.ioManager))
                {
                    currentList.Add(io.attachedIO.ioManager);
                    io.attachedIO.ioManager.RecursiveGetConveyorGroup(currentList, getInputs);
                }
            }*/
        }

        /// <summary>
        /// Changes the state of a <see cref="Conveyor"/>
        /// </summary>
        /// <param name="state">The new state for the <see cref="Conveyor"/></param>
        public void ChangeConveyorState(bool state)
        {
            if (!isConveyor)
                return;

            int newSpeed;

            if (state)
                newSpeed = 1;
            else
                newSpeed = 0;

            mc.conveyor.speed = newSpeed;
        }

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

        #region Misc

        /// <summary>
        /// Determines whether a <see cref="BuildingIOManager"/> contains a <see cref="BuildingIO"/>
        /// </summary>
        /// <param name="io">The <see cref="BuildingIO"/> the <see cref="BuildingIOManager"/> might contain</param>
        /// <returns>Whether the <see cref="BuildingIOManager"/> contains the <see cref="BuildingIO"/></returns>
        public bool ContainsIO(BuildingIO io)
        {
            bool contains = false;

            IOForEach(managerIO =>
            {
                if (managerIO.Equals(io)) contains = true;
            });

            return contains;
        }

        /// <summary>
        /// Visualizes the colliders each IO uses for other IO detection (seen in Scene view)
        /// </summary>
        public void VisualizeColliders()
        {
            //IOForEach(io => io.DrawIODetectionBox());
        }

        // Gizmos should only be drawn while in the editor
#if UNITY_EDITOR

        // If using the wire grid code uncomment
        //private Vector3Int buildingSize = Vector3Int.zero;

        /// <summary>
        /// Renders IOs for visualization when setting up
        /// </summary>
        private void OnDrawGizmos()
        {
            // Only draw if not in game
            if (Application.isPlaying)
            {
                // If using the wire grid code uncomment
                //if (buildingSize == Vector3.zero)
                //{
                //    Vector3 e = transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size;
                //    buildingSize = new Vector3Int(Mathf.RoundToInt(e.x), Mathf.RoundToInt(e.y), Mathf.RoundToInt(e.z));
                //}

                Gizmos.color = Color.red;

                // Code for drawing a wire grid of the building size
                //for (int x = 0; x < buildingSize.x; x++)
                //{
                //    for (int y = 0; y < buildingSize.y; y++)
                //    {
                //        for (int z = 0; z < buildingSize.z; z++)
                //        {
                //            Gizmos.DrawWireCube(new Vector3(0.5f - x, 0.5f - y, 0.5f - z), new Vector3(1f, 1f, 1f));
                //        }
                //    }
                //}



                // Draw inputs
                Gizmos.color = new Color(0, 0.47f, 1);
                foreach (BuildingIO input in inputs)
                {
                    Vector3 cubePosition = new Vector3(0.5f - input.localPosition.x, 0.5f, 0.5f - input.localPosition.y);

                    if (Application.isPlaying) cubePosition += mc.building.meshData.pos;

                    Vector3 direction = input.direction.GetDirection(); // Because the input arrow needs to be facing inwards, the arrow needs to go the opposite direction.

                    DrawBuildingArrow(cubePosition, direction, true);
                }

                // Draw outputs
                Gizmos.color = new Color(1, 0.64f, 0);
                foreach (BuildingIO output in outputs)
                {
                    Vector3 cubePosition = new Vector3(0.5f - output.localPosition.x, 0.5f, 0.5f - output.localPosition.y);
                    Vector3 direction = output.direction.GetDirection();

                    if (Application.isPlaying) cubePosition += mc.building.meshData.pos;

                    DrawBuildingArrow(cubePosition, direction);
                }

                // Reset color
                Gizmos.color = Color.white;
            }
        }

        /// <summary>
        /// Draws the visual arrow for a <see cref="BuildingIO"/> while <see cref="OnDrawGizmos"/> is running
        /// </summary>
        /// <param name="cubePosition">The position of the cube</param>
        /// <param name="direction">The direction of the input or output</param>
        /// <param name="reversed">If the bool is true, the arrow will be drawn in the position of the opposite direction but still FACING the same direction</param>
        private void DrawBuildingArrow(Vector3 cubePosition, Vector3 direction, bool reversed = false)
        {
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
    }
}
