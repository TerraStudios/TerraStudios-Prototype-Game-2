//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System.Collections.Generic;
using System.Linq;
using BuildingManagement;
using CoreManagement;
using ItemManagement;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using Utilities;

namespace BuildingModules
{
    /// <summary>
    /// Class that contains all needed variables for an item to be processed for item movement.
    /// </summary>
    public class ConveyorItemData
    {
        public ItemData data;
        public Transform sceneInstance;
        public bool reachedEnd;
    }

    [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct ItemMovementJob : IJobParallelForTransform
    {
        public float speed;
        public float3 endPos;
        public float deltaTime;

        [NativeDisableParallelForRestriction] public NativeList<int> reachedEnd;

        /// <summary>
        /// Called when a job is being executed.
        /// </summary>
        /// <param name="index">Defines which itteration is currently being done. Defined by <c>itemsOnTop.Count</c> when scheduling the job.</param>
        /// <param name="transform">Transform of that item to move.</param>
        public void Execute(int index, TransformAccess transform)
        {
            float3 toPos = MoveTowards(transform.position, endPos, deltaTime * speed);
            if (toPos.Equals(endPos))
                reachedEnd.Add(index);
            else
                transform.position = new Vector3(toPos.x, toPos.y, toPos.z);
        }

        /// <summary>
        /// Burst-ready version of Vector3.MoveTowards. Much faster.
        /// </summary>
        /// <param name="current">Current position</param>
        /// <param name="target">Target position</param>
        /// <param name="maxDistanceDelta">Time Delta</param>
        /// <returns>New position to move to.</returns>
        public float3 MoveTowards(float3 current, float3 target, float maxDistanceDelta)
        {
            float deltaX = target.x - current.x;
            float deltaY = target.y - current.y;
            float deltaZ = target.z - current.z;
            float sqdist = deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ;

            if (sqdist == 0 || sqdist <= maxDistanceDelta * maxDistanceDelta)
                return target;

            float dist = (float)math.sqrt(sqdist);

            return new float3(current.x + deltaX / dist * maxDistanceDelta,
                current.y + deltaY / dist * maxDistanceDelta,
                current.z + deltaZ / dist * maxDistanceDelta);
        }
    }

    /// <summary>
    /// This class handles the conveyor behaviour and is placed on each conveyor.
    /// </summary>
    public class Conveyor : MonoBehaviour, IConveyorBase
    {
        public ModuleConnector mc;
        public float speed = 0.005f;

        public List<ConveyorItemData> itemsOnTop = new List<ConveyorItemData>();

        private Vector3 startMovePos;
        private Vector3 endOfBeltPos;

        public Vector3 EndOfBeltPos
        {
            get
            {
                return math.lerp(startMovePos, endOfBeltPos, 0.5f);
            }

            set
            {
                endOfBeltPos = value;
            }
        }

        //private GameObject statusSphere;

        /// <summary>
        /// Called when the building begins to be initialized.
        /// </summary>
        public void Init()
        {
            mc.buildingIOManager.onItemEnterInput.AddListener(OnItemEnterBelt);

            // Input 0 and output 0 always correspond to the start and end points no matter
            // the building direction.

            BuildingIO input = mc.buildingIOManager.inputs[0];
            BuildingIO output = mc.buildingIOManager.outputs[0];

            startMovePos = mc.buildingIOManager.GetTargetIOPosition(input) + Vector3.down * 0.25f;
            EndOfBeltPos = mc.buildingIOManager.GetTargetIOPosition(output) + Vector3.down * 0.25f;

            /*GameObject sphereInput = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereInput.transform.position = startMovePos;
            sphereInput.transform.localScale /= 2;
            sphereInput.GetComponent<Renderer>().material.color = Color.green;
            GameObject sphereOutput = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereOutput.transform.position = endMovePos;
            sphereOutput.transform.localScale /= 2;
            sphereOutput.GetComponent<Renderer>().material.color = Color.red;*/

            //statusSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //statusSphere.transform.position = mc.building.meshData.pos + Vector3.up * 0.5f;
            //statusSphere.transform.localScale /= 2;
        }

        /// <summary>
        /// Called when an item enters the belt.
        /// </summary>
        /// <param name="itemEnterInfo"></param>
        private void OnItemEnterBelt(OnItemEnterEvent itemEnterInfo)
        {
            ConveyorItemData data;

            if (!itemEnterInfo.sceneInstance)
            {
                data = new ConveyorItemData()
                {
                    data = itemEnterInfo.item,
                    sceneInstance = ObjectPoolManager.Instance.ReuseObject(itemEnterInfo.item.obj.gameObject, startMovePos, Quaternion.identity).transform
                };
            }
            else
            {
                data = new ConveyorItemData()
                {
                    data = itemEnterInfo.item,
                    sceneInstance = itemEnterInfo.sceneInstance.transform
                };
            }

            itemsOnTop.Add(data);
        }

        // Conveyor Item Movement job variables
        ItemMovementJob job;
        JobHandle movementJobHandle;
        TransformAccessArray accessArray;
        NativeList<int> reachedEndArray;

        /// <summary>
        /// Method for efficiently updating conveyors. Same as MonoBehaviour.Update() but more efficient.
        /// </summary>
        public void UpdateConveyor()
        {
            /*if (IsBusy())
                statusSphere.GetComponent<Renderer>().material.color = Color.red;
            else
                statusSphere.GetComponent<Renderer>().material.color = Color.green;*/

            if (itemsOnTop.Count == 0)
                return;

            accessArray = new TransformAccessArray(itemsOnTop.Count, GameManager.Instance.conveyorDesiredJobCount);
            reachedEndArray = new NativeList<int>(itemsOnTop.Count, Allocator.TempJob);

            job = new ItemMovementJob()
            {
                speed = speed,
                endPos = EndOfBeltPos,
                deltaTime = Time.deltaTime,
                reachedEnd = reachedEndArray
            };

            for (int i = 0; i < itemsOnTop.Count; i++)
            {
                Transform tr = itemsOnTop[i].sceneInstance;
                accessArray.Add(tr);
                tr.gameObject.SetActive(mc.building.isVisible);
            }

            movementJobHandle = job.Schedule(accessArray);
        }

        /// <summary>
        /// Method for efficiently updating conveyors. Same as MonoBehaviour.LateUpdate() but more efficient.
        /// </summary>
        public void LateUpdateConveyor()
        {
            if (reachedEndArray.IsCreated) // Avoid NRE if the belt has not performed a job yet.
            {
                // Complete the job now. This way the movement is smooth and happens in only one frame.
                movementJobHandle.Complete();

                if (!reachedEndArray.IsEmpty) // Check if anything needs to be processed
                {
                    foreach (int i in reachedEndArray)
                    {
                        ConveyorItemData data = itemsOnTop.ElementAtOrDefault(i);

                        // This most probably has to be reworked
                        if (data == null)
                            continue;

                        data.reachedEnd = true; // Mark the item that it reached the end so we can process it later 

                        if (mc.buildingIOManager.ConveyorMoveNext(data)) // Check and move item to the next belt
                        {
                            itemsOnTop.Remove(data); // Item successfully passed, removed it from itemsOnTop
                        }
                    }
                }

                // Dispose the NativeArrays after finishing the job.
                accessArray.Dispose();
                reachedEndArray.Dispose();
            }
        }

        /// <summary>
        /// Shows the items that are current flowing in this <c>Conveyor</c>.
        /// </summary>
        public void LoadItemMeshes()
        {
            for (int i = 0; i < itemsOnTop.Count; i++)
            {
                Transform sceneInstance = itemsOnTop[i].sceneInstance;

                sceneInstance.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Hides the items that are current flowing in this <c>Conveyor</c>
        /// </summary>
        public void UnloadItemMeshes()
        {
            for (int i = 0; i < itemsOnTop.Count; i++)
            {
                Transform sceneInstance = itemsOnTop[i].sceneInstance;

                if (sceneInstance)
                    sceneInstance.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Removes an item from <c>itemsOnTop</c> when an item successfully moves to the next conveyor.
        /// </summary>
        /// <param name="sceneInstance">Scene Instance GameObject of that item.</param>
        public void RemoveItemFromBelt(GameObject sceneInstance, bool destroySceneInstance = false)
        {
            itemsOnTop.Remove(itemsOnTop.Single(i => i.sceneInstance == sceneInstance.transform));

            if (destroySceneInstance)
                ObjectPoolManager.Instance.DestroyObject(sceneInstance);
        }

        /// <summary>
        /// Returns whether the belt is busy.
        /// </summary>
        public bool IsBusy()
        {
            if (itemsOnTop.Count > 0)
                return true;
            else
                return false;
        }
    }
}
