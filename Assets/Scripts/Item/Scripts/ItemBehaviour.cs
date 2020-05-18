using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    public ItemData data;

    private void OnTriggerStay(Collider other)
    {
        BuildingIO bIO = other.GetComponent<BuildingIO>();
        if (bIO != null && IsFullySubmerged(other) && bIO.isInput)
            bIO.OnItemEnter(this);
    }

    private void OnTriggerExit(Collider other)
    {
        BuildingIO bIO = other.GetComponent<BuildingIO>();
        if (bIO != null)
            bIO.OnItemExit(this);
    }

    private bool IsFullySubmerged(Collider bc) 
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Bounds bounds = bc.bounds;
        foreach (Vector3 vec in vertices)
        {
            Vector3 worldPos = transform.TransformPoint(vec);
            if (bounds.Contains(worldPos) == false) { return false; }
        }
        return true;
    }
}