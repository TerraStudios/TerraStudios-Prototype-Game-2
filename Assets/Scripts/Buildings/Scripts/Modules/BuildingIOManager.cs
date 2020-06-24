﻿using System.Collections.Generic;
using UnityEngine;

public class BuildingIOManager : MonoBehaviour
{
    public Building Building;
    public ItemData itemInside;

    public BuildingIO[] inputs;
    public BuildingIO[] outputs;

    public bool debug;

    [Header("Conveyor Properties")]
    public bool isConveyor;
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
        foreach (BuildingIOManager bIO in GetInputConveyorGroup(inputID))
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

    private List<BuildingIOManager> GetInputConveyorGroup(int? inputID)
    {
        List<BuildingIOManager> toReturn = new List<BuildingIOManager>();

        BuildingIOManager next;
        if (inputID != null)
            next = inputs[inputID.Value].myManager;
        else
            next = this;

        foreach (BuildingIO io in next.inputs)
        {
            if (io.attachedIO)
            {
                toReturn.Add(io.attachedIO.myManager);
                toReturn.AddRange(io.attachedIO.myManager.GetInputConveyorGroup(null));
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