using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// Event args
public struct OnItemEnterArgs 
{
    public int inputID;
    public ItemData item;
    public GameObject sceneInstance;
}

public class BuildingIOManager : MonoBehaviour
{
    public ModuleConnector mc;
    public List<ItemData> itemsInside = new List<ItemData>();

    public BuildingIO[] inputs;
    public BuildingIO[] outputs;

    public bool debug;

    [Header("Conveyor Properties")]
    public bool isConveyor;
    public Conveyor[] ConveyorManagers;

    [Header("Events")]
    public UnityEvent<OnItemEnterArgs> OnItemEnterInput;

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

    public void ProceedItemEnter(GameObject sceneInstance, ItemData item, int inputID)
    {
        if (debug)
        {
            Destroy(sceneInstance);
            outputs[0].SpawnItemObj(item);
            return;
        }

        if (itemsInside.Contains(item))
            return;

        Destroy(sceneInstance, 1f);
        itemsInside.Add(item);

        OnItemEnterArgs args = new OnItemEnterArgs()
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
        foreach (BuildingIOManager bIO in GetConveyorGroup(inputID))
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