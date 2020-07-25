using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// Event args
public class OnItemEnterEvent : UnityEvent<OnItemEnterEvent>
{
    public int inputID;
    public ItemData item;
    public GameObject sceneInstance;
}

public class ItemInsideData
{
    public int quantity;
    public ItemData item;
}

public class BuildingIOManager : MonoBehaviour
{
    public ModuleConnector mc;
    public List<ItemInsideData> itemsInside = new List<ItemInsideData>();

    [Tooltip("A list of all the BuildingIO inputs for the building")]
    public BuildingIO[] inputs;
    [Tooltip("A list of all the BuildingIO outputs for the building")]
    public BuildingIO[] outputs;

    [Header("Conveyor Properties")]
    public bool isConveyor;

    //I have no idea what this is
    public Conveyor[] ConveyorManagers;

    [Header("Events")]
    public OnItemEnterEvent OnItemEnterInput;

    /// <summary>
    /// Initializes all of the <see cref="Building">'s <see cref="BuildingIO"/>s and calls the <see cref="OnItemEnterEvent"/> event. 
    /// </summary>
    public void Init()
    {
        IOForEach(io => io.Init());

        

        OnItemEnterInput = new OnItemEnterEvent();
    }

    /// <summary>
    /// Updates the position of the arrow <see cref="GameObject"/> to the current IO's position. 
    /// 
    /// Currently used for updating the position of any arrows on the visualization (which can be moved).
    /// </summary>
    public void UpdateArrows()
    {
        IOForEach(io =>
        {
            if (io.arrow)
            {
                io.arrow.position = io.transform.position + new Vector3(0, 1, 0);
                io.arrow.rotation = io.transform.rotation;
            }
        });
    }

    public void UpdateIOPhysics() 
    {
        IOForEach(io =>
        {
            io.OnVisualizationMoved(mc.Building);
        });
    }

    /// <summary>
    /// Signals every <see cref="BuildingIO"/> in the building to attempt a link with any other attached <see cref="BuildingIO"/>s.
    /// </summary>
    public void LinkAll()
    {
        IOForEach(io => io.MakeLink());
    }

    public void ProceedItemEnter(GameObject sceneInstance, ItemData item, int inputID)
    {
        Destroy(sceneInstance, 1f);

        ItemInsideData occurrence = itemsInside.FirstOrDefault(found => found.item.ID == item.ID);
        if (occurrence != null) // there's already an item with the same ID in the list
        {
            occurrence.quantity++; // just change its quantity
        }
        else // this item is new
        {
            ItemInsideData toAdd = new ItemInsideData()
            {
                quantity = 1,
                item = item
            };
            itemsInside.Add(toAdd);
        }

        OnItemEnterEvent args = new OnItemEnterEvent()
        {
            inputID = inputID,
            item = item,
            sceneInstance = sceneInstance
        };
        OnItemEnterInput.Invoke(args);

        Debug.Log("Item fully in me! Item is " + item.name);
    }

    public void TrashItem(GameObject sceneInstance, ItemData item)
    {
        Destroy(sceneInstance, 1f);
        BuildingIO trashOutput = GetTrashOutput();
        trashOutput.SpawnItemObj(item);
    }

    /// <summary>
    /// Signals every <see cref="BuildingIO"> in the building to create a blue arrow <see cref="GameObject"/> visualization above it.
    /// </summary>
    public void VisualizeAll()
    {
        IOForEach(io => io.VisualizeArrow(BuildingManager.instance.blueArrow));
    }

    /// <summary>
    /// Signals every <see cref="BuildingIO"> in the building to stop visualizing.
    /// </summary>
    public void DevisualizeAll()
    {
        IOForEach(io => io.Devisualize());
    }

    // FIX!!!!
    /*public string GetItemInsideName()
    {
        if (itemInside)
            return itemInside.name;
        else
            return "None";
    }*/

    /// <summary>
    /// Retrieves the <see cref="BuildingIO"/> marked with <see cref="BuildingIO.isTrashcanOutput"/> 
    /// </summary>
    /// <returns>The found <see cref="BuildingIO"/>, or null if no trash output is found</returns>
    public BuildingIO GetTrashOutput()
    {
        foreach (BuildingIO output in outputs)
        {
            if (output.isTrashcanOutput)
                return output;
        }
        return null;
    }

    /// <summary>
    /// Modifies the conveyor group belonging to a <see cref="Building"/> and sets each one to a given state
    /// </summary>
    /// <param name="state">The new <see cref="WorkStateEnum"/> for the conveyor group to be in</param>
    public void SetConveyorGroupState(WorkStateEnum state)
    {
        foreach (BuildingIOManager bIO in GetConveyorGroup())
        {
            bIO.mc.Building.SetWorkstateSilent(state); //set it silently to not trigger on workstate changed (recursion)
        }
    }

    /// <summary>
    /// Retrieves the conveyor group of any building ID.
    /// </summary>
    /// <returns>A <see cref="List{T}"/> of <see cref="BuildingIOManager"/>s</returns>
    private List<BuildingIOManager> GetConveyorGroup()
    {

        List<BuildingIOManager> toReturn = new List<BuildingIOManager>();
        RecursiveGetConveyorGroup(toReturn, true);
        //Debug.Log(isConveyor);
        if (isConveyor) RecursiveGetConveyorGroup(toReturn, false);

        Debug.Log($"Returned a conveyor group of size {toReturn.Count}");

        return toReturn;
    }


    /// <summary>
    /// Recursive method for retrieving the conveyor group of any Building ID. Works by adding all of the attached IOs of a building and its attached IOs as well (until a non conveyor is found or there isn't another attached IO)
    /// </summary>
    /// <param name="currentList">A list of <see cref="BuildingIOManager"/>s that will end up being the result. Passed through as a pointer reference.</param>
    /// <param name="getInputs">Determines whether it should also search through the building's inputs as well</param>
    /// <returns>A <see cref="List{T}"/> of <see cref="BuildingIOManager"></see> in the <paramref name="getInputs"/> parameter</returns>
    private void RecursiveGetConveyorGroup(List<BuildingIOManager> currentList, bool getInputs = true)
    {
        if (getInputs)
        {
            foreach (BuildingIO io in inputs)
            {
                if (io.attachedIO && io.attachedIO.IOManager.isConveyor && !currentList.Contains(io.attachedIO.IOManager))
                {
                    currentList.Add(io.attachedIO.IOManager);
                    io.attachedIO.IOManager.RecursiveGetConveyorGroup(currentList, true);
                }
            }
        }
        else // also return all outputs
        {
            foreach (BuildingIO io in outputs) // borked
            {
                if (io.attachedIO && io.attachedIO.IOManager.isConveyor && !currentList.Contains(io.attachedIO.IOManager))
                {
                    Debug.Log("Found attached IO");
                    currentList.Add(io.attachedIO.IOManager); //add itself
                    io.attachedIO.IOManager.RecursiveGetConveyorGroup(currentList, false); //add children
                }
            }
        }
    }

    /// <summary>
    /// Changes the state of a <see cref="Conveyor"/>
    /// </summary>
    /// <param name="state">The new state for the <see cref="Conveyor"/></param>
    public void ChangeConveyorState(bool state)
    {
        if (!isConveyor)
            return;

        int newSpeed;

        if (state)
            newSpeed = 1;
        else
            newSpeed = 0;

        foreach (Conveyor conv in ConveyorManagers)
        {
            conv.speed = newSpeed;
        }
    }

    /// <summary>
    /// Utility method for looping through both the inputs and outputs
    /// </summary>
    /// <param name="action">Delegate for the action taken in each IO</param>
    private void IOForEach(Action<BuildingIO> action)
    {
        foreach (BuildingIO io in inputs)
        {
            action(io);
        }

        foreach (BuildingIO io in outputs)
        {
            action(io);
        }
    }


    #region Misc

    public bool ContainsIO(BuildingIO io)
    {
        bool contains = false;

        IOForEach(managerIO =>
        {
            if (managerIO.Equals(io)) contains = true;
        });

        return contains;
    }

    /// <summary>
    /// Visualizes the colliders each IO uses for other IO detection (seen in Scene view)
    /// </summary>
    public void VisualizeColliders()
    {
        IOForEach(io => io.DrawIODetectionBox());
    }

    #endregion
}