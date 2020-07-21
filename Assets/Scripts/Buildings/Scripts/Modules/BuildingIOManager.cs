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
    /// <param name="inputID">Optional parameter for a specific building's ID to start at</param>
    /// <param name="state">The new state for the conveyor</param>
    public void ModifyConveyorGroup(int? inputID, bool state)
    {
        foreach (BuildingIOManager bIO in GetConveyorGroup(inputID, state))
        {
            if (state)
            {
                bIO.mc.Building.WorkState = WorkStateEnum.On;
            }
            else
            {
                bIO.mc.Building.WorkState = WorkStateEnum.Off;
            }
        }
    }

    /// <summary>
    /// Retrieves the conveyor group of any building ID.
    /// </summary>
    /// <param name="inputID">The optional building ID of where to start</param>
    /// <param name="state">The new state for the conveyor</param>
    /// <param name="getInputs">Determines whether it should also search through the building's inputs as well</param>
    /// <returns>A <see cref="List{T}"/> of <see cref="BuildingIOManager"/>s</returns>
    private List<BuildingIOManager> GetConveyorGroup(int? inputID, bool state, bool getInputs = true)
    {
        List<BuildingIOManager> toReturn = new List<BuildingIOManager>();
        toReturn.AddRange(RecursiveGetConveyorGroup(inputID, state, getInputs));
        //Debug.Log(isConveyor);
        if (isConveyor) toReturn.AddRange(RecursiveGetConveyorGroup(inputID, state, !getInputs));

        return toReturn;
    }


    /// <summary>
    /// Recursive method for retrieving the conveyor group of any Building ID. Works by adding all of the attached IOs of a building and its attached IOs as well (until a non conveyor is found or there isn't another attached IO)
    /// </summary>
    /// <param name="inputID">The optional building ID of where to start</param>
    /// <param name="state">The new state for the conveyor</param>
    /// <param name="getInputs">Determines whether it should also search through the building's inputs as well</param>
    /// <returns>A <see cref="List{T}"/> of <see cref="BuildingIOManager"</returns>
    private List<BuildingIOManager> RecursiveGetConveyorGroup(int? inputID, bool state, bool getInputs = true)
    {
        List<BuildingIOManager> toReturn = new List<BuildingIOManager>();

        BuildingIOManager next;
        if (inputID != null)
        {
            if (!isConveyor)
                next = inputs[inputID.Value].myManager;
            else
                next = outputs[inputID.Value].myManager;
        }
        else
            next = this;

        if (getInputs)
        {
            foreach (BuildingIO io in next.inputs)
            {
                if (io.attachedIO && io.attachedIO.myManager.isConveyor && io.attachedIO.myManager.mc.Building.WorkState != (state ? WorkStateEnum.On : WorkStateEnum.Off))
                {
                    toReturn.Add(io.attachedIO.myManager);
                    toReturn.AddRange(io.attachedIO.myManager.RecursiveGetConveyorGroup(null, true));
                }
            }
        }
        else // also return all outputs
        {
            foreach (BuildingIO io in next.outputs) // borked
            {
                if (io.attachedIO && io.attachedIO.myManager.isConveyor && io.attachedIO.myManager.mc.Building.WorkState != (state ? WorkStateEnum.On : WorkStateEnum.Off))
                {
                    Debug.Log("Found attached IO");
                    toReturn.Add(io.attachedIO.myManager); //add itself
                    toReturn.AddRange(io.attachedIO.myManager.RecursiveGetConveyorGroup(null, false)); //add children
                }
            }
        }

        return toReturn;
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
}