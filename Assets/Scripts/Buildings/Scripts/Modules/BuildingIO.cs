using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingIO : MonoBehaviour
{
    public bool isInput;
    public bool isOutput;
    public MeshRenderer MeshRenderer;
    public Collider coll;
    public BuildingIOManager myManager;

    [Header("Input Configuration")]
    public ItemData[] itemsAllowedToEnter;
    public ItemCategories[] itemCategoriesAllowedToEnter;

    [Header("Dynamic variables")]
    public BuildingIO attachedIO;
    [HideInInspector] public bool visualizeIO = true;

    private void OnTriggerEnter(Collider other)
    {
        OnUpdateIO(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnUpdateIO(other, true);
    }

    public void Init()
    {
        visualizeIO = false;
    }

    public void Visualize()
    {
        if (!visualizeIO)
            Visualize(Color.blue);
    }

    private void OnUpdateIO(Collider other, bool exit = false) 
    {
        BuildingIO hit = other.GetComponent<BuildingIO>();

        if (hit == null || hit == this)
            return;

        bool isInputUnsupported = IsInputUnsupported(hit);

        if (visualizeIO) 
        {
            if (isInputUnsupported)
                Visualize(Color.red);
            else
                Visualize(Color.green);
        }
            
        else if (!hit.visualizeIO && !isInputUnsupported)
        {
            Debug.Log("Attached " + hit);
            attachedIO = hit;
        }
    }

    private bool IsInputUnsupported(BuildingIO other) 
    {
        bool toReturn = false;

        if (!myManager.isConveyor && !other.myManager.isConveyor)
            toReturn = true;
        if (isInput && other.isInput)
            toReturn = true;
        if (isOutput && other.isOutput)
            toReturn = true;

        return toReturn;
    }

    public void OnItemEnter(ItemBehaviour item)
    {
        bool allowedToEnter = false;

        foreach(ItemData data in itemsAllowedToEnter)
        {
            if (item.data.ID == data.ID)
                allowedToEnter = true;
        }

        foreach(ItemCategories data in itemCategoriesAllowedToEnter)
        {
            if (item.data.ItemCategory == data)
                allowedToEnter = true;
        }

        if (!allowedToEnter)
            return;

        myManager.ProceedItemEnter(item.gameObject, item.data);
    }

    public void OnItemExit(ItemBehaviour item)
    {
        Debug.Log("Item exit " + item.data.name);
        //! probably not needed
    }

    private void Visualize(Color color)
    {
        MeshRenderer.enabled = true;
        MeshRenderer.material.color = color;
    }

    public void Devisualize()
    {
        MeshRenderer.enabled = false;
    }
}
