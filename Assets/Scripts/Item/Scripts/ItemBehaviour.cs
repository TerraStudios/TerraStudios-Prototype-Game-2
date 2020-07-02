using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    public ItemData data;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(12)) 
        {
            BuildingIO bIO = other.transform.parent.GetComponent<BuildingIO>();
            if (bIO.isInput && !bIO.myManager.isConveyor)
                bIO.OnItemEnter(this);
        } 
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer.Equals(9))
        {
            BuildingIO bIO = other.GetComponent<BuildingIO>();
            bIO.OnItemExit(this);
        } 
    }
}