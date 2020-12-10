using System;
using System.Collections.Generic;
using Tayx.Graphy.Utils.NumString;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BuildingModules;
using BuildingManagers;
using DebugTools;
using EconomySystem;
using TimeSystem;

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
        if (removeModeEnabled)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                foreach (ItemBehaviour t in inRange.Item1)
                {
                    if (!DeleteItem(t.data, t.gameObject))
                        return;
                }

                foreach (Building b in inRange.Item2)
                {
                    if (!DeleteBuilding(b))
                        return;
                }

                inRange = new Tuple<List<ItemBehaviour>, List<Building>>(new List<ItemBehaviour>(), new List<Building>());
            }

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
    }

    public void OnRemoveButtonPressed()
    {
        removePanel.SetActive(true);
        removeModeEnabled = true;
        BuildingManager.Instance.OnBuildingDeselected();
        TimeEngine.IsPaused = true;
    }

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

    private void SaveInRange(Vector3 snappedPos)
    {
        lastSnappedPos = snappedPos;

        List<ItemBehaviour> itemsToReturn = new List<ItemBehaviour>();
        List<Building> buildingsToReturn = new List<Building>();

        Vector3 scale = new Vector3() { x = (brushSize.value + 1) / 2 - 0.1f, y = 2, z = (brushSize.value + 1) / 2 - 0.1f };
        ExtDebug.DrawBox(snappedPos, scale, Quaternion.identity, Color.red);

        if (RemoveBuildings)
            foreach (Collider hit in Physics.OverlapBox(snappedPos, scale, Quaternion.identity, buildingLayer))
            {
                buildingsToReturn.Add(hit.transform.GetComponent<Building>());
            }

        if (RemoveItems)
            foreach (Collider hit in Physics.OverlapBox(snappedPos, scale, Quaternion.identity, itemsLayer))
            {
                itemsToReturn.Add(hit.transform.GetComponent<ItemBehaviour>());
            }

        inRange = new Tuple<List<ItemBehaviour>, List<Building>>(itemsToReturn, buildingsToReturn);
    }

    private Vector3 GetSnappedPos()
    {
        RaycastHit? gridHit = GridManager.Instance.FindGridHit();
        if (gridHit == null) return default;
        Vector3 snappedPos = GridManager.Instance.GetGridPosition(gridHit.Value.point, new Vector2Int() { x = brushSize.value.ToInt() + 1, y = brushSize.value.ToInt() + 1 });
        if (snappedPos == lastSnappedPos) return default;
        return snappedPos;
    }

    public bool DeleteBuilding(Building b)
    {
        decimal change = (decimal)((float)b.bBase.healthPercent / 100 * b.bBase.Price - (b.bBase.Price * GameManager.Profile.removePenaltyMultiplier));
        EconomyManager.Instance.Balance += change;

        b.mc.buildingIOManager.DevisualizeAll();
        b.mc.buildingIOManager.UnlinkAll();

        if (b.mc.buildingIOManager.isConveyor)
        {
            ConveyorManager.Instance.conveyors.Remove(b.mc.conveyor);
        }

        foreach (KeyValuePair<ItemData, int> item in b.mc.buildingIOManager.itemsInside)
        {
            for (int i = 0; i < item.Value; i++)
            {
                DeleteItem(item.Key);
            }
        }

        foreach (BuildingIO io in b.mc.buildingIOManager.outputs)
        {
            foreach (ItemSpawnData item in io.itemsToSpawn)
            {
                DeleteItem(item.itemToSpawn);
            }
        }

        BuildingSystem.UnRegisterBuilding(b);
        Destroy(b.gameObject); // Destroy game object
        return true;
    }

    public bool DeleteItem(ItemData data, GameObject obj = null)
    {
        //Debug.Log($"Adding {data.startingPriceInShop * GameManager.removePenaltyMultiplier} to the balance.");
        if (data.isGarbage)
        {
            decimal change = (decimal)(data.StartingPriceInShop + (data.StartingPriceInShop * GameManager.Profile.garbageRemoveMultiplier));
            EconomyManager.Instance.Balance += change;
        }
        else
        {
            decimal change = (decimal)(data.StartingPriceInShop - (data.StartingPriceInShop * GameManager.Profile.removePenaltyMultiplier));
            EconomyManager.Instance.Balance += change;
        }

        if (obj)
            ObjectPoolManager.Instance.DestroyObject(obj); //destroy object
        return true;
    }

    public void OnBrushSliderValueChanged()
    {
        float brushSizeToDisplay = brushSize.value + 1;
        brushText.text = brushSizeToDisplay + "x" + brushSizeToDisplay;
    }

    // subtract == true => Decrease value by 1
    // subtract == false => Increase value by 1
    public void OnManualBrushSizeChange(bool subtract)
    {
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