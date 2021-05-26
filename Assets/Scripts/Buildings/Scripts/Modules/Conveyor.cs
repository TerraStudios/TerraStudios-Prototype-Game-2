//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BuildingManagement;
using ItemManagement;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using Utilities;

namespace BuildingModules
{
    public class ConveyorItemData
    {
        public ItemData data;
        public Transform sceneInstance;
        public bool reachedEnd;
    }

    [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low, DisableSafetyChecks = true)]
    public struct ItemMovementJob : IJobParallelForTransform
    {
        public float speed;
        public float3 endPos;
        public float deltaTime;

        [NativeDisableParallelForRestriction] public NativeList<int> reachedEnd;

        public void Execute(int index, TransformAccess transform)
        {
            float3 toPos = MoveTowards(transform.position, endPos, deltaTime * speed);
            if (toPos.Equals(endPos))
                reachedEnd.Add(index);
            else
                transform.position = new Vector3(toPos.x, toPos.y, toPos.z);
        }

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
        private Vector3 endMovePos;

        public void Init()
        {
            mc.buildingIOManager.OnItemEnterInput.AddListener(OnItemEnterBelt);

            // Input 0 and output 0 always correspond to the start and end points no matter
            // the building direction.

            BuildingIO input = mc.buildingIOManager.inputs[0];
            BuildingIO output = mc.buildingIOManager.outputs[0];

            startMovePos = mc.buildingIOManager.GetTargetIOPosition(input);
            endMovePos = mc.buildingIOManager.GetTargetIOPosition(output);

            /*GameObject sphereInput = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereInput.transform.position = startMovePos;
            sphereInput.transform.localScale /= 2;
            sphereInput.GetComponent<Renderer>().material.color = Color.green;
            GameObject sphereOutput = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereOutput.transform.position = endMovePos;
            sphereOutput.transform.localScale /= 2;
            sphereOutput.GetComponent<Renderer>().material.color = Color.red;*/
        }

        private void OnItemEnterBelt(OnItemEnterEvent itemEnterInfo)
        {
            BuildingIO io = mc.buildingIOManager.outputs[0];
            if (io.linkedIO != null)
            {
                if (!io.linkedIO.manager.mc.conveyor.IsBusy())
                {
                    //Debug.Log("No item inside next belt, continuing...");
                }
                else
                {
                    //Debug.Log("Item inside next belt detected, stopping");
                    return;
                }
            }

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
            Debug.Log("Added item to itemsOnTop");
        }

        ItemMovementJob job;
        JobHandle movementJobHandle;
        TransformAccessArray accessArray;
        NativeList<int> reachedEndArray;

        /// <summary>
        /// Method for efficiently updating conveyors. Same as MonoBehaviour.Update() but more efficient.
        /// </summary>
        public void UpdateConveyor()
        {
            if (itemsOnTop.Count == 0)
                return;

            accessArray = new TransformAccessArray(itemsOnTop.Count);
            reachedEndArray = new NativeList<int>(itemsOnTop.Count, Allocator.TempJob);

            job = new ItemMovementJob()
            {
                speed = speed,
                endPos = endMovePos,
                deltaTime = Time.deltaTime,
                reachedEnd = reachedEndArray
            };

            for (int i = 0; i < itemsOnTop.Count; i++)
            {
                accessArray.Add(itemsOnTop[i].sceneInstance);
            }

            movementJobHandle = job.Schedule(accessArray);
        }

        /// <summary>
        /// Method for efficiently updating conveyors. Same as MonoBehaviour.LateUpdate() but more efficient.
        /// </summary>
        public void LateUpdateConveyor()
        {
            if (reachedEndArray.IsCreated)
            {
                movementJobHandle.Complete();
                if (!reachedEndArray.IsEmpty)
                {
                    foreach (int i in reachedEndArray)
                    {
                        ConveyorItemData data = itemsOnTop.ElementAtOrDefault(i);

                        // TODO: This eliminates memory leak when itemsOnTop is begin accessed without thi IF and the Editor freezes
                        // This most probably has to be reworked
                        if (data == null)
                            continue;

                        data.reachedEnd = true; // mark that the item reached the end so we can later process it

                        if (mc.buildingIOManager.ConveyorMoveNext(data)) // check and move item to the next belt
                        {
                            itemsOnTop.Remove(data); // item successfully passed, removed it from itemsOnTop
                        }
                    }
                }

                accessArray.Dispose();
                reachedEndArray.Dispose();
            }
        }

        public bool IsBusy()
        {
            // TODO: Redesign according to GDD
            /*if (itemsOnTop.Count > 0) // HERE: check why when this is true, after this it doesn't continue accepting items
                return true;
            else
                return false;
            */
            return false;
        }
    }
}
