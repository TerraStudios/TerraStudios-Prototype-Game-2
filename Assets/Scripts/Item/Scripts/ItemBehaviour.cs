using UnityEngine;

[ExecuteInEditMode]
public class ItemBehaviour : MonoBehaviour
{
    public ItemData data;
    private BuildingIO insideIO;

    private Material originalMaterial;
    private bool markedForDelete;

    private void Awake()
    {
        originalMaterial = GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void MarkForDelete()
    {
        if (markedForDelete)
            return;
        markedForDelete = true;
        GetComponent<MeshRenderer>().material = BuildingManager.instance.redArrow;
    }

    public void UnmarkForDelete()
    {
        if (!markedForDelete)
            return;
        markedForDelete = false;
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
        UnmarkForDelete();
    }
}
