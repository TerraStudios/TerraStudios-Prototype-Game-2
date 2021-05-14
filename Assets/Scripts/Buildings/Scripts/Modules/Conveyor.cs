//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System.Collections;
using System.Collections.Generic;
using BuildingManagement;
using ItemManagement;
using UnityEngine;

namespace BuildingModules
{
    public class ConveyorItemData
    {
        public ItemData data;
        public float progress;
    }

    /// <summary>
    /// This class handles the conveyor behaviour and is placed on each conveyor.
    /// </summary>
    public class Conveyor : MonoBehaviour, IConveyorBase
    {
        public ModuleConnector mc;
        public float speed = 1;

        public List<ConveyorItemData> itemsOnTop = new List<ConveyorItemData>();

        private Vector3 startMovePos;
        private Vector3 endMovePos;

        public void Init()
        {
            mc.buildingIOManager.OnItemEnterInput.AddListener(OnItemEnterBelt);

            BuildingIO input = mc.buildingIOManager.inputs[0];
            BuildingIO output = mc.buildingIOManager.outputs[0];

            startMovePos = mc.buildingIOManager.GetIOPosition(input);
            endMovePos = mc.buildingIOManager.GetIOPosition(output);

            GameObject sphereInput = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereInput.transform.position = startMovePos;
            sphereInput.transform.localScale /= 2;
            GameObject sphereOutput = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereOutput.transform.position = endMovePos;
            sphereOutput.transform.localScale /= 2;
        }

        private void OnItemEnterBelt(OnItemEnterEvent itemEnterInfo)
        {
            // check if next belt has an item inside
            // access item scene instance
            // move it, update progress by applying speed

            if (mc.buildingIOManager.inputs[0].linkedIO.manager.HasItemInside())
            {
                Debug.Log("No item inside next belt, continuing...");

            }
            else
            {
                Debug.Log("Item inside next belt detected, stopping");
            }
        }

        /// <summary>
        /// Method for efficiently updating conveyors. Same as MonoBehaviour.Update() but more efficient.
        /// </summary>
        public void UpdateConveyor()
        {
            // Apply progress
            for (int i = 0; i < itemsOnTop.Count; i++)
            {
                itemsOnTop[i].progress += speed * Time.unscaledDeltaTime;
            }

            // Visualize Progress

            // Posibility 1
            // Update progress in regular special Conveyor Update (fast)
            // Update visualization by running a job - gets progress and shows it visually
            // if progress == 1, move item to next belt

            // Posibility 2
            // Make a task that handles all position updates, including time and speed for one item.
        }
    }
}
