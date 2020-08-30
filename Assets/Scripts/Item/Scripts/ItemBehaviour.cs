using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ItemBehaviour : MonoBehaviour
{
    public ItemData data;
    private BuildingIO insideIO;

    public Material originalMaterial;
    public bool markedForDelete;

    public void MarkForDelete()
    {
        if (markedForDelete)
            return;
        markedForDelete = true;
        originalMaterial = GetComponent<MeshRenderer>().material;
        GetComponent<MeshRenderer>().material = BuildingManager.instance.redArrow;
    }

    public void UnmarkForDelete()
    {
        if (!markedForDelete)
            return;
        GetComponent<MeshRenderer>().material = originalMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(12))
        {
            insideIO = other.transform.parent.GetComponent<BuildingIO>();
            insideIO.itemInside = this;
            if (insideIO.isInput && !insideIO.IOManager.isConveyor)
                insideIO.OnItemEnter(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer.Equals(12))
        {
            BuildingIO bIO = other.transform.parent.GetComponent<BuildingIO>();
            bIO.itemInside = null;
            if (bIO.isInput && !bIO.IOManager.isConveyor)
                bIO.OnItemExit(this);
            insideIO = null;
        }
    }

    private void OnDisable()
    {
        if (insideIO)
            insideIO.itemInside = null;
    }
}