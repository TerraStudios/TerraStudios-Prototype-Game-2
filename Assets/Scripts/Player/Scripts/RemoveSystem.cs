using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Tayx.Graphy.Utils.NumString;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class RemoveSystem : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject RemovePanel;
    public Slider brushSize;

    [Header("Components")]
    public GridManager GridManager;

    [Header("Variables")]
    public LayerMask buildingLayer;
    public LayerMask itemsLayer;
    public float itemRemoveMultiplier;
    public float buildingRemoveMultiplier;

    [Header("Dynamic variables")]
    private bool removeModeEnabled;
    private Tuple<List<ItemBehaviour>, List<Building>> inRange = new Tuple<List<ItemBehaviour>, List<Building>>(new List<ItemBehaviour>(), new List<Building>());

    public static RemoveSystem instance;

    public bool RemoveBuildings { get; set; } = true;
    public bool RemoveItems { get; set; } = true;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (removeModeEnabled)
        {
            foreach (ItemBehaviour t in inRange.Item1)
                t.UnmarkForDelete();
            foreach (Building b in inRange.Item2)
                b.UnmarkForDelete();
/*
            if (inRange.)
                for (int i = 0; i < inRange.Item1.Count; i++)
                {

                }

            for (int i = 0; i < inRange.Item2.Count; i++)
            {

            }*/

            SaveInRange();

            foreach (ItemBehaviour t in inRange.Item1)
                t.MarkForDelete();
            foreach (Building b in inRange.Item2)
                b.MarkForDelete();

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                foreach (ItemBehaviour t in inRange.Item1)
                    DeleteItem(t.data, t.gameObject);
                foreach (Building b in inRange.Item2)
                    DeleteBuilding(b);
            }
        }
    }

    public void OnRemoveButtonPressed() 
    {
        RemovePanel.SetActive(true);
        removeModeEnabled = true;
        BuildingManager.instance.OnBuildingDeselected();
    }

    public void OnDisableRemoveButtonPressed() 
    {
        RemovePanel.SetActive(false);
        removeModeEnabled = false;
    }

    private void SaveInRange()
    {
        RaycastHit? gridHit = GridManager.instance.FindGridHit();
        if (gridHit == null) return;

        Vector3 snappedPos = GridManager.instance.GetGridPosition(gridHit.Value.point, new Vector2Int() { x = brushSize.value.ToInt() / 2, y = brushSize.value.ToInt() / 2 });

        List<ItemBehaviour> itemsToReturn = new List<ItemBehaviour>();
        List<Building> buildingsToReturn = new List<Building>();

        Vector3 scale = new Vector3() { x = brushSize.value / 2, y = 2, z = brushSize.value / 2 };

        if (RemoveBuildings)
            foreach (RaycastHit hit in Physics.BoxCastAll(snappedPos, scale, transform.forward, Quaternion.identity, 100, buildingLayer))
            {
                buildingsToReturn.Add(hit.transform.GetComponent<Building>());
            }
                
        if (RemoveItems)
            foreach (RaycastHit hit in Physics.BoxCastAll(snappedPos, scale, transform.forward, Quaternion.identity, 100, itemsLayer))
            {
                itemsToReturn.Add(hit.transform.GetComponent<ItemBehaviour>());
            }
                

        Debug.Log($"Found + { itemsToReturn.Count + buildingsToReturn.Count }");

        inRange = new Tuple<List<ItemBehaviour>, List<Building>>(itemsToReturn, buildingsToReturn);
    }

    public void DeleteBuilding(Building b)
    {
        b.mc.BuildingIOManager.DevisualizeAll();
        b.mc.BuildingIOManager.UnlinkAll();

        if (b.mc.BuildingIOManager.isConveyor)
        {
            ConveyorManager.instance.conveyors.Remove(b.mc.Conveyor);
        }

        Debug.Log($"Adding {b.healthPercent * b.price - b.price * buildingRemoveMultiplier} to the balance.");
        EconomyManager.instance.Balance += (decimal)(b.healthPercent * b.price - b.price * buildingRemoveMultiplier);

        BuildingSystem.RegisteredBuildings.Remove(b);
        Destroy(b.gameObject); // Destroy game object
    }

    public void DeleteItem(ItemData data, GameObject obj)
    {
        Debug.Log($"Adding {data.startingPriceInShop * itemRemoveMultiplier} to the balance.");
        EconomyManager.instance.Balance += (decimal)(data.startingPriceInShop * itemRemoveMultiplier);
        ObjectPoolManager.instance.DestroyObject(obj); //destroy object 
    }
}
