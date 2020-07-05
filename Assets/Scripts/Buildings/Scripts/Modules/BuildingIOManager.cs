using System.Collections.Generic;
using UnityEngine;

public class BuildingIOManager : MonoBehaviour
{
    [Tooltip("Reference to the building the BuildingIOManager is attached to ")]
    public Building Building;

    [Tooltip("The current item inside the building")]
    public ItemData itemInside;

    [Tooltip("A list of all the BuildingIO inputs for the building")]
    public BuildingIO[] inputs;
    [Tooltip("A list of all the BuildingIO outputs for the building")]
    public BuildingIO[] outputs;

    [Tooltip("Determines whether debug logs should be active during gameplay")]
    public bool debug;

    [Header("Conveyor Properties")]
    public bool isConveyor;

    //I have no idea what this is 
    public Conveyor[] ConveyorManagers;

    public void Init()
    {
        foreach (BuildingIO io in inputs)
        {
            io.Init();
        }

        foreach (BuildingIO io in outputs)
        {
            io.Init();
        }
    }

    public void ProceedItemEnter(GameObject sceneInstance, ItemData item)
    {
        if (debug)
        {
            Destroy(sceneInstance);
            outputs[0].SpawnItemObj(item);
            return;
        }

        if (item == itemInside)
            return;

        Destroy(sceneInstance, 1f);
        itemInside = item;

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
            io.Visualize();
        }

        foreach (BuildingIO io in outputs)
        {
            io.Visualize();
        }
    }

    public void DevisualizeAll()
    {
        foreach (BuildingIO io in inputs)
        {
            io.Devisualize();
        }

        foreach (BuildingIO io in outputs)
        {
            io.Devisualize();
        }
    }

    public string GetItemInsideName()
    {
        if (itemInside)
            return itemInside.name;
        else
            return "None";
    }

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
        foreach (BuildingIOManager bIO in GetConveyorGroup(inputID))
        {
            if (state)
            {
                bIO.Building.WorkState = WorkStateEnum.On;
            }
            else
            {
                bIO.Building.WorkState = WorkStateEnum.Off;
            }
        }
    }

    private List<BuildingIOManager> GetConveyorGroup(int? inputID, bool getInputs = true)
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
                if (io.attachedIO)
                {
                    toReturn.Add(io.attachedIO.myManager);
                    toReturn.AddRange(io.attachedIO.myManager.GetConveyorGroup(null, true));
                }
            }
        }

        if (isConveyor) // also return all outputs
        {
            foreach (BuildingIO io in next.outputs) // borked
            {
                if (io.attachedIO)
                {
                    toReturn.Add(io.attachedIO.myManager);
                    toReturn.AddRange(io.attachedIO.myManager.GetConveyorGroup(null, false));
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

        foreach(Conveyor conv in ConveyorManagers)
        {
            conv.speed = newSpeed;
        }
    }
}