//
// Developed by TerraStudios (https://github.com/TerraStudios)
//
// Copyright(c) 2020-2021 Konstantin Milev (konstantin890 | milev109@gmail.com)
// Copyright(c) 2020-2021 Yerti (UZ9)
//
// The following script has been written by either konstantin890 or Yerti (UZ9) or both.
// This file is covered by the GNU GPL v3 license. Read LICENSE.md for more information.
// Past NDA/MNDA and Confidential notices are revoked and invalid since no agreement took place. Read README.md for more information.
//

using System;
using BuildingManagement;
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
            GetComponent<MeshRenderer>().material = BuildingManager.Instance.redColor;
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
