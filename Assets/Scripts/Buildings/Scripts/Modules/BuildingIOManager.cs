﻿//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using BuildingManagement;
using DebugTools;
using ItemManagement;
using System.Collections;
using System.Collections.Generic;
using TerrainGeneration;
using TerrainTypes;
using UnityEngine;
using Utilities;

namespace BuildingModules
{
    /// <summary>
    /// Manages the Building IO system.
    /// </summary>
    public class BuildingIOManager : BuildingIOSystem
    {
        #region Initializers
        /// <summary>
        /// Initializes all of the <see cref="Building">'s <see cref="BuildingIO"/>s and calls the <see cref="OnItemEnterEvent"/> event. 
        /// </summary>
        public void Init()
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i].manager = this;
                inputs[i].id = i;
            }

            for (int i = 0; i < outputs.Length; i++)
            {
                outputs[i].manager = this;
                outputs[i].id = i;
            }

            buildingOffset = new Vector3();

            Vector3 buildingSize;

            if (!Application.isPlaying)
            {
                buildingSize = transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size;
            }
            else
            {
                buildingSize = mc.building.correspondingMesh.GetComponent<MeshRenderer>().bounds.size;
            }


            Vector3Int size = new Vector3Int(
                Mathf.CeilToInt(Mathf.Round(buildingSize.x * 10f) / 10f),
                Mathf.CeilToInt(Mathf.Round(buildingSize.y * 10f) / 10f),
                Mathf.CeilToInt(Mathf.Round(buildingSize.z * 10f) / 10f));

            buildingOffset.x = size.x % 2 != 0 ? -0.5f : 0;
            buildingOffset.z = size.z % 2 != 0 ? -0.5f : 0;

            OnItemEnterInput = new OnItemEnterEvent();
        }

        private void Update()
        {
            if (GridManager.Instance.debugMode)
            {
                IOForEach(io =>
                {
                    if (io.manager == null) return;

                    ExtDebug.DrawVoxel(GetTargetIOPosition(io), Color.green);
                });
            }
        }

        #endregion

        #region IO Linking

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

        /// <summary>
        /// Attempts to make a "link" between two BuildingIOs
        /// </summary>
        /// <param name="io">The input to attempt to connect to (current building).</param>
        /// <param name="input">Whether the IO is input or output true = input, false = output.</param>
        private void AttemptLink(BuildingIO io, bool input = true)
        {
            // Retrieves the OPPOSITE voxel (normal vector with a magnitude of 1 voxel)
            Vector3 linkVoxelPos = GetTargetIOPosition(io);
            Voxel targetvoxel = TerrainGenerator.Instance.GetVoxel(linkVoxelPos.FloorToInt3());

            Quaternion buildingRot = io.manager.mc.building.meshData.rot;

            if (targetvoxel is MachineSlaveVoxel voxel)
            {
                BuildingIOManager targetBuilding = voxel.controller.mc.buildingIOManager;

                Quaternion targetBuildingRot = targetBuilding.mc.building.meshData.rot;

                // Loop through opposite of io's type
                foreach (BuildingIO targetIO in input ? targetBuilding.outputs : targetBuilding.inputs)
                {
                    // 1st Check: Get the position of the voxel perpendicular to the target IO, and check if it equals the desired linkVoxelPos
                    // 2nd Check: Make sure the directions are opposite by making sure the two direction vectors equal zero
                    if (targetBuilding.GetIOPosition(targetIO) == linkVoxelPos && io.direction.GetDirection(buildingRot) + (targetIO.direction.GetDirection(targetBuildingRot)) == Vector3Int.zero)
                    {
                        // Found successful link, set linkedIO for both
                        targetIO.linkedIO = io;
                        io.linkedIO = targetIO;

                        Debug.Log("Successfully linked");
                    }
                }
            }

        }

        #endregion

        #region Item Handlers (legacy)

        /*public void ProceedItemEnter(GameObject sceneInstance, ItemData item, int inputID)
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
        }*/

        #endregion

        #region Item Handling (new)

        /// <summary>
        /// Called when an <see cref="ItemData"/> attempts to enter a <see cref="BuildingIO"/>.
        /// </summary>
        /// <param name="data">The Item that attempts to enter.</param>
        /// <param name="inputID">The input ID from which it attempts to enter.</param>
        public void AttemptItemEnter(ItemData data, int inputID, GameObject sceneInstance)
        {
            Debug.Log("[Item Enter] Item attempts to enter! Item is " + data.name);

            OnItemEnterEvent args = new OnItemEnterEvent()
            {
                inputID = inputID,
                item = data,
                sceneInstance = sceneInstance
            };

            OnItemEnterInput.Invoke(args);
        }

        public void AddItem(ItemData data)
        {
            if (itemsInside.ContainsKey(data))
            {
                itemsInside[data]++;
            }
            else
            {
                // This probably won't work lol.
                itemsInside[data] = 1;
            }

            Debug.Log("Item " + data.name + " stored inside building!");
        }

        /// <summary>
        /// Called when an <see cref="ItemData"/> is ejected/dropped out of a Building
        /// </summary>
        /// <param name="data"></param>
        public void EjectItem(ItemData data, int outputID, bool removeFromItems = true, float timeToSpawn = 1)
        {
            // maybe add an event in the future

            // check if item exists inside
            // if it does, remove it from the items inside. if not - drop a log
            // invoke some kind of an event

            if (removeFromItems)
            {
                if (itemsInside.ContainsKey(data))
                {
                    RemoveItem(data);
                }
                else
                {
                    Debug.LogError("Attempting to remove an item when ejecting, though" +
                        "the item doesn't exist inside the building!");
                }
            }

            // queue goes here

            ItemQueueData spawnData = new ItemQueueData()
            {
                outputID = outputID,
                item = data,
                timeToSpawn = timeToSpawn
            };

            itemsToSpawn.Enqueue(spawnData);

            if (itemsToSpawn.Count == 1)
                ExecuteSpawn(spawnData);

            Debug.Log("Item " + data.name + " added to the ejection queue of the building!");
        }

        /// <summary>
        /// Called when an item finishes moving in a belt.
        /// </summary>
        /// <param name="conveyorItemData">Special conveyor move data.</param>
        /// <returns>Whether the item moves to the next belt.</returns>
        public bool ConveyorMoveNext(ConveyorItemData conveyorItemData)
        {
            BuildingIO attachedIO = GetAttachedToBelt();

            if (attachedIO == null)
                return false;

            if (attachedIO.manager.isConveyor)
            {
                // Attached IO is a belt

                if (attachedIO.manager.mc.conveyor.IsBusy())
                    return false;

                //Debug.Log("Next IO is conveyor, moving item to it!");
                OnItemEnterEvent args = new OnItemEnterEvent()
                {
                    inputID = 0, // not sure if it's good to hard code this...
                    item = conveyorItemData.data,
                    sceneInstance = conveyorItemData.sceneInstance.gameObject
                };

                attachedIO.manager.OnItemEnterInput.Invoke(args);

                return true;
            }
            else
            {
                // Attached IO is a building

                //Debug.Log("Next IO is a building, attempting item enter!");
                //ObjectPoolManager.Instance.DestroyObject(conveyorItemData.sceneInstance.gameObject);
                attachedIO.AttemptIOEnter(conveyorItemData.data, conveyorItemData.sceneInstance.gameObject);
                return true;
            }
        }

        private void ExecuteSpawn(ItemQueueData queueData)
        {
            StartCoroutine(ProcessSpawn(queueData));
        }

        private IEnumerator ProcessSpawn(ItemQueueData queueData)
        {
            yield return new WaitForSeconds(queueData.timeToSpawn);
            yield return new WaitWhile(GetAttachedToBelt().manager.mc.conveyor.IsBusy);

            BuildingIO target = outputs[queueData.outputID].linkedIO;
            if (target != null)
            {
                if (queueData.sceneInstance)
                    target.AttemptIOEnter(queueData.item, queueData.sceneInstance);
                else
                    target.AttemptIOEnter(queueData.item, null);
            }

            /* // Legacy code, rethink whether it is needed
             * 
             * //An item has been instantiated, attempt to allow APM (if present) to insert an item
            if (ioManager.mc.apm)
            {
                IOForEach(io =>
                {
                    if (io.isInput && io.itemInside)
                    {
                        ProceedItemEnter(io.itemInside.gameObject, io.itemInside.data, io.ID);
                    }
                });
            }*/

            FinishSpawn();
        }

        private void FinishSpawn()
        {
            itemsToSpawn.Dequeue();

            if (itemsToSpawn.Count != 0)
            {
                ItemQueueData next = itemsToSpawn.Peek();
                ExecuteSpawn(next);
            }
        }

        public void RemoveItem(ItemData data)
        {
            // maybe add an event in the future

            itemsInside.Remove(data);
            Debug.Log("Item " + data.name + " removed from the building!");
        }

        #endregion

        #region Visualization Handlers (to rewrite)

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
        /// Visualizes the colliders each IO uses for other IO detection (seen in Scene view)
        /// </summary>
        public void VisualizeColliders()
        {
            // NOTE (by Kosio): If this is going to be low-level code, move to BuildingIOSystem.cs
            // TODO: Update with new code here
            //IOForEach(io => io.DrawIODetectionBox());
        }

        /// <summary>
        /// Signals every <see cref="BuildingIO"> in the building to create a blue arrow <see cref="GameObject"/> visualization above it.
        /// </summary>
        public void VisualizeAll()
        {
            // NOTE (by Kosio): If this is going to be low-level code, move to BuildingIOSystem.cs
            // TODO: Update with new code here
            //IOForEach(io => io.VisualizeArrow());
        }

        /// <summary>
        /// Signals every <see cref="BuildingIO"> in the building to stop visualizing.
        /// </summary>
        public void DevisualizeAll()
        {
            // NOTE (by Kosio): If this is going to be low-level code, move to BuildingIOSystem.cs
            // TODO: Update with new code here
            //IOForEach(io => io.Devisualize());
        }

        #endregion

        #region Conveyor

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

        #endregion
    }
}
