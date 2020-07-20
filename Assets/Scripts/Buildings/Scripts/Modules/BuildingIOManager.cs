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
    public int quanity;
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

    public void Init()
    {
        IOForEach(io => io.Init());
        Debug.Log($"Inputs: {inputs.Length}, Outputs: {outputs.Length}");

        OnItemEnterInput = new OnItemEnterEvent();
    }

    public void MarkForLinking()
    {
        IOForEach(io => io.ReadyToLink = true);
    }

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
            occurrence.quanity++; // just change its quantity
        }
        else // this item is new
        {
            ItemInsideData toAdd = new ItemInsideData()
            {
                quanity = 1,
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

    public void VisualizeAll()
    {
        foreach (BuildingIO io in inputs)
        {
            io.VisualizeArrow(BuildingManager.instance.blueArrow);
        }

        foreach (BuildingIO io in outputs)
        {
            io.VisualizeArrow(BuildingManager.instance.blueArrow);
        }
    }

    public void DevisualizeAll()
    {
        Debug.Log("Devisualizing item");

        Debug.Log("There are " + (inputs.Length + outputs.Length) + " IOs");

        foreach (BuildingIO io in inputs)
        {
            Debug.Log("Found correct IO");
        }

        foreach (BuildingIO io in outputs)
        {
            Debug.Log("Found correct IO");
        }

        IOForEach(io =>
        {
            io.Devisualize();
            Debug.Log("Found an IO");
        });
    }

    // FIX!!!!
    /*public string GetItemInsideName()
    {
        if (itemInside)
            return itemInside.name;
        else
            return "None";
    }*/

    public BuildingIO GetTrashOutput()
    {
        foreach (BuildingIO output in outputs)
        {
            if (output.isTrashcanOutput)
                return output;
        }
        return null;
    }

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

    private List<BuildingIOManager> GetConveyorGroup(int? inputID, bool state, bool getInputs = true)
    {
        List<BuildingIOManager> toReturn = new List<BuildingIOManager>();
        toReturn.AddRange(RecursiveGetConveyorGroup(inputID, state, getInputs));
        //Debug.Log(isConveyor);
        if (isConveyor) toReturn.AddRange(RecursiveGetConveyorGroup(inputID, state, !getInputs));

        return toReturn;
    }



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