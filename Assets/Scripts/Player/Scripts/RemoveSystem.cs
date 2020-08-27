using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

    [Header("Dynamic variables")]
    public bool removeBuildings = true;
    public bool removeItems = true;
    private bool removeModeEnabled;

    public static RemoveSystem instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (removeModeEnabled)
        {
            // get all colliding buildings, store them and when clicked, remove all of them
            if (Input.GetMouseButtonDown(0))
                GetInRange(GridManager.instance.GetGridPosition(Input.mousePosition));
        }
    }

    public void OnRemoveButtonPressed() 
    {
        RemovePanel.SetActive(true);
        removeModeEnabled = true;
    }

    public void OnDisableRemoveButtonPressed() 
    {
        RemovePanel.SetActive(false);
        removeModeEnabled = false;
    }

    private List<Transform> GetInRange(Vector3 snappedPos)
    {
        List<Transform> toReturn = new List<Transform>();

        Vector3 scale = new Vector3() { x = brushSize.value / 2, y = 2, z = brushSize.value / 2 };

        if (removeBuildings)
            foreach (RaycastHit hit in Physics.BoxCastAll(snappedPos, scale, transform.forward, Quaternion.identity, 100, buildingLayer))
                toReturn.Add(hit.transform);
        if (removeItems)
            foreach (RaycastHit hit in Physics.BoxCastAll(snappedPos, scale, transform.forward, Quaternion.identity, 100, itemsLayer))
                toReturn.Add(hit.transform);

        return toReturn;
    }
}
