//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using BuildingManagement;
using BuildingModules;
using CoreManagement;
using DebugTools;
using EconomyManagement;
using ItemManagement;
using Tayx.Graphy.Utils.NumString;
using TerrainGeneration;
using TerrainTypes;
using TimeSystem;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utilities;

namespace Player
{
    /// <summary>
    /// The RemoveSystem removes items and buildings placed in the world.
    /// </summary>
    public class RemoveSystem : MonoBehaviour
    {
        [Header("UI Components")]
        public GameObject removePanel;
        public Slider brushSize;
        public TMP_Text brushText;

        [Header("Components")]
        public GridManager gridManager;

        [Header("Variables")]
        public LayerMask buildingLayer;
        public LayerMask itemsLayer;

        [Header("Dynamic variables")]
        public bool removeModeEnabled;

        private Tuple<List<ItemBehaviour>, List<Building>> inRange = new Tuple<List<ItemBehaviour>, List<Building>>(new List<ItemBehaviour>(), new List<Building>());

        public static RemoveSystem instance;

        public bool RemoveBuildings { get; set; } = true;
        public bool RemoveItems { get; set; } = true;

        private Vector3 lastSnappedPos;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (!removeModeEnabled)
                return;

            Vector3 snappedPos = GetSnappedPos();

            // check if we're at the same position
            if (snappedPos.Equals(default))
                return;

            // unmark for delete all previous items/buildings
            foreach (ItemBehaviour t in inRange.Item1)
                t.UnmarkForDelete();
            foreach (Building b in inRange.Item2)
                b.UnmarkForDelete();

            // store new items/buildings inside
            SaveInRange(snappedPos);

            // mark for delete all of them
            foreach (ItemBehaviour t in inRange.Item1)
                t.MarkForDelete();
            foreach (Building b in inRange.Item2)
                b.MarkForDelete();
        }

        /// <summary>
        /// Triggered when the button to remove building/items is pressed.
        /// </summary>
        /// <param name="context"></param>
        public void RemoveTrigger(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (removeModeEnabled && !EventSystem.current.IsPointerOverGameObject())
                {
                    DeleteItems();

                    foreach (Building b in inRange.Item2)
                    {
                        DeleteBuilding(b);
                    }

                    inRange = new Tuple<List<ItemBehaviour>, List<Building>>(new List<ItemBehaviour>(), new List<Building>());
                }
            }
        }

        /// <summary>
        /// Triggered when the remove button is pressed. This enables the remove system.
        /// </summary>
        public void OnRemoveButtonPressed()
        {
            removePanel.SetActive(true);
            removeModeEnabled = true;
            BuildingManager.Instance.OnBuildingDeselected();
            TimeEngine.IsPaused = true;
        }

        /// <summary>
        /// Triggered when the remove system is attempted to be stopped.
        /// </summary>
        public void OnDisableRemoveButtonPressed()
        {
            removePanel.SetActive(false);
            removeModeEnabled = false;

            foreach (ItemBehaviour t in inRange.Item1)
                t.UnmarkForDelete();
            foreach (Building b in inRange.Item2)
                b.UnmarkForDelete();

            inRange = new Tuple<List<ItemBehaviour>, List<Building>>(new List<ItemBehaviour>(), new List<Building>());
            TimeEngine.IsPaused = false;
        }

        /// <summary>
        /// Saves buildings and items in the range specified.
        /// </summary>
        /// <param name="snappedPos"></param>
        private void SaveInRange(Vector3 snappedPos)
        {
            lastSnappedPos = snappedPos;

            List<ItemBehaviour> itemsToReturn = new List<ItemBehaviour>();
            List<Building> buildingsToReturn = new List<Building>();

            Vector3 scale = new Vector3((brushSize.value + 1) / 2 - 0.1f, 2, (brushSize.value + 1) / 2 - 0.1f);
            ExtDebug.DrawBox(snappedPos, scale, Quaternion.identity, Color.red);

            int3 initialPos = snappedPos.FloorToInt3();

            float radius = (brushSize.value + 1) / 2f;

            int min = Mathf.FloorToInt(-radius) + 1;
            int max = Mathf.FloorToInt(radius);

            HashSet<Building> buildingsToRemove = new HashSet<Building>();

            for (int y = min; y <= max; y++)
            {
                for (int x = min; x <= max; x++)
                {
                    for (int z = min; z <= max; z++)
                    {
                        Voxel voxel = TerrainGenerator.Instance.GetVoxel(new int3(initialPos.x + x, initialPos.y + y,
                                initialPos.z + z));

                        switch (voxel)
                        {
                            case null:
                                continue; // Voxel was out of bounds
                            case MachineSlaveVoxel slaveVoxel:
                                // We found a voxel that belongs to a building, go ahead and add it to the hashset
                                buildingsToRemove.Add(slaveVoxel.controller);
                                break;

                        }
                    }
                }
            }

            //if (RemoveBuildings) buildingsToReturn.AddRange(Physics.OverlapBox(snappedPos, scale, Quaternion.identity, buildingLayer).Select(hit => hit.transform.GetComponent<Building>()));
            if (RemoveItems) itemsToReturn.AddRange(Physics.OverlapBox(snappedPos, scale, Quaternion.identity, itemsLayer).Select(hit => hit.transform.GetComponent<ItemBehaviour>()));

            inRange = new Tuple<List<ItemBehaviour>, List<Building>>(itemsToReturn, buildingsToRemove.ToList());
        }

        /// <summary>
        /// Retrieves the snapped grid position from the mouse position.
        /// </summary>
        /// <returns>Snapped Grid position.</returns>
        private Vector3 GetSnappedPos()
        {
            RaycastHit? gridHit = GridManager.Instance.FindGridHit();
            if (gridHit == null) return default;
            Vector3 snappedPos = GridManager.Instance.GetGridPosition(gridHit.Value.point, new Vector3Int() { x = brushSize.value.ToInt() + 1, y = brushSize.value.ToInt() + 1, z = 1 });
            if (snappedPos == lastSnappedPos) return default;
            return snappedPos;
        }

        /// <summary>
        /// Delete a building manually. Arguments are usually the ones that were collected from <c>SaveInRange</c>.
        /// </summary>
        /// <param name="b">Building to delete</param>
        public void DeleteBuilding(Building b)
        {
            if (!RemoveBuildings)
                return;

            decimal change = (decimal)((float)b.bBase.healthPercent / 100 * b.bBase.Price - (b.bBase.Price * GameManager.Instance.CurrentGameProfile.removePenaltyMultiplier));
            EconomyManager.Instance.Balance += change;

            b.mc.buildingIOManager.DestroyArrows();
            b.mc.buildingIOManager.UnlinkAll();

            if (b.mc.buildingIOManager.isConveyor)
            {
                ConveyorManager.Instance.conveyors.Remove(b.mc.conveyor);
            }

            // Delete all items inside the building
            foreach (KeyValuePair<ItemData, int> item in b.mc.buildingIOManager.itemsInside)
            {
                for (int i = 0; i < item.Value; i++)
                {
                    DeleteItem(item.Key);
                }
            }

            // Delete all items pending to be outputted
            foreach (BuildingIO io in b.mc.buildingIOManager.outputs)
            {
                foreach (ItemQueueData item in io.itemsToSpawn)
                {
                    DeleteItem(item.item);
                }
            }

            // If conveyor, delete items on top
            if (b.mc.buildingIOManager.isConveyor)
            {
                foreach (ConveyorItemData item in b.mc.conveyor.itemsOnTop.ToList())
                {
                    b.mc.conveyor.RemoveItemFromBelt(item.sceneInstance.gameObject, true);
                }
            }

            BuildingSystem.UnRegisterBuilding(b);
            BuildingSystem.RelinkLinkedIOs(b);
            ObjectPoolManager.Instance.DestroyObject(b.correspondingMesh.gameObject);
            Destroy(b.gameObject); // Destroy game object
        }

        /// <summary>
        /// Deletes all items collected from <c>SaveInRange</c>.
        /// </summary>
        public void DeleteItems()
        {
            foreach (Building b in inRange.Item2)
            {
                if (b.mc.buildingIOManager.isConveyor)
                {
                    foreach (ConveyorItemData item in b.mc.conveyor.itemsOnTop.ToList())
                    {
                        //Debug.Log($"Adding {data.startingPriceInShop * GameManager.removePenaltyMultiplier} to the balance.");
                        if (item.data.isGarbage)
                        {
                            decimal change = (decimal)(item.data.StartingPriceInShop + (item.data.StartingPriceInShop * GameManager.Instance.CurrentGameProfile.garbageRemoveMultiplier));
                            EconomyManager.Instance.Balance += change;
                        }
                        else
                        {
                            decimal change = (decimal)(item.data.StartingPriceInShop - (item.data.StartingPriceInShop * GameManager.Instance.CurrentGameProfile.removePenaltyMultiplier));
                            EconomyManager.Instance.Balance += change;
                        }

                        b.mc.conveyor.RemoveItemFromBelt(item.sceneInstance.gameObject, true);
                    }
                }
            }
        }

        /// <summary>
        /// Delete an item manually. Usually called when attempting to delete an item that's inside a building (with or without scene instance).
        /// </summary>
        /// <param name="data">Item Data of the item</param>
        /// <param name="obj">Scene Instance of the item, if applies.</param>
        public void DeleteItem(ItemData data, GameObject obj = null)
        {
            if (data.isGarbage)
            {
                decimal change = (decimal)(data.StartingPriceInShop + (data.StartingPriceInShop * GameManager.Instance.CurrentGameProfile.garbageRemoveMultiplier));
                EconomyManager.Instance.Balance += change;
            }
            else
            {
                decimal change = (decimal)(data.StartingPriceInShop - (data.StartingPriceInShop * GameManager.Instance.CurrentGameProfile.removePenaltyMultiplier));
                EconomyManager.Instance.Balance += change;
            }

            if (obj)
                ObjectPoolManager.Instance.DestroyObject(obj); //destroy object
        }

        /// <summary>
        /// Called from the UI when the value of the Brush slider is changed.
        /// </summary>
        public void OnBrushSliderValueChanged()
        {
            float brushSizeToDisplay = brushSize.value + 1;
            brushText.text = brushSizeToDisplay + "x" + brushSizeToDisplay;
        }

        /// <summary>
        /// Called when one of the buttons (+ or -) is pressed in the UI.
        /// </summary>
        /// <param name="subtract">Whether it should add or subtract to the brush size.</param>
        public void OnManualBrushSizeChange(bool subtract)
        {
            // subtract == true => Decrease value by 1
            // subtract == false => Increase value by 1

            float newBrushSize;
            if (subtract)
            {
                newBrushSize = brushSize.value - 1;
                if (newBrushSize < brushSize.minValue)
                    newBrushSize = 0;
            }
            else
            {
                newBrushSize = brushSize.value + 1;
                if (newBrushSize > brushSize.maxValue)
                    newBrushSize = brushSize.maxValue;
            }

            brushSize.value = newBrushSize;
            brushText.text = (newBrushSize + 1) + "x" + (newBrushSize + 1);
        }
    }
}
