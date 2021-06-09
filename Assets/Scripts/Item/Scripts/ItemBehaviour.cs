//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using BuildingManagement;
using BuildingModules;
using UnityEngine;

namespace ItemManagement
{
    /// <summary>
    /// This script is placed on each Item GameObjects.
    /// Its purpose is to hold properties of Items as well as handling physics events.
    /// </summary>
    [SerializeField]
    [ExecuteInEditMode]
    public class ItemBehaviour : MonoBehaviour
    {
        public ItemData data;
        //private BuildingIO insideIO;

        [NonSerialized] private Material originalMaterial;
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
            GetComponent<MeshRenderer>().material = BuildingManager.Instance.redArrow;
        }

        public void UnmarkForDelete()
        {
            if (!markedForDelete)
                return;
            markedForDelete = false;
            GetComponent<MeshRenderer>().material = originalMaterial;
        }

        private void OnDisable()
        {
            //if (insideIO)
            //    insideIO.itemInside = null;
            UnmarkForDelete();
        }
    }
}
