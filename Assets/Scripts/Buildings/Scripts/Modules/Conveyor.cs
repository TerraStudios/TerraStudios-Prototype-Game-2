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

        public void Init()
        {
            mc.buildingIOManager.OnItemEnterInput.AddListener(OnItemEnterBelt);
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

        }
    }
}
