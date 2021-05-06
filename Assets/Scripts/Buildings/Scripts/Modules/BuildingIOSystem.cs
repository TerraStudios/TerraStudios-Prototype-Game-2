using System;
using System.Collections;
using System.Collections.Generic;
using BuildingModules;
using ItemManagement;
using UnityEngine;
using UnityEngine.Events;

namespace BuildingModules
{
    public class BuildingIOSystem : MonoBehaviour
    {
        
    }

    // Bellow are classes and enums necessary for the IO System.

    [Serializable]
    public class BuildingIO
    {
        [NonSerialized] [HideInInspector] public BuildingIOManager manager; // Needed for accessing data on the building via the IO

        public Vector2 localPosition;
        public IODirection direction;

        //TODO: Remove this?
        private IOType type;
        public bool isTrashcanOutput;

        [HideInInspector]
        public BuildingIO linkedIO; // The IO this BuildingIO is connected to, e.g. an input BuildingIO to an output
    }

    /// <summary>
    /// The enum is used to store the direction of an IO, as items can only input or output from one face of a building.
    /// </summary>
    public enum IODirection
    {
        Forward,
        Backward,
        Left,
        Right
    }

    [Serializable]
    public enum IOType
    {
        Input,
        Output
    }

    /// <summary>
    /// Properties for the event when an item attempts to 'enter' a Building.
    /// </summary>
    public class OnItemEnterEvent : UnityEvent<OnItemEnterEvent>
    {
        public int inputID;
        public ItemData item;
        public GameObject sceneInstance;
        public Dictionary<ItemData, int> proposedItems;
    }

    /// <summary>
    /// Properties of an item that is present 'inside' the Building.
    /// </summary>
    public class ItemInsideData
    {
        public int quantity;
        public ItemData item;
    }
}
