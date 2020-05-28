using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingIOManager : MonoBehaviour
{


    public ItemData itemInside;

    public BuildingIO[] inputs;
    public BuildingIO[] outputs;

    public bool isConveyor;

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
        foreach(BuildingIO output in outputs)
        {
            if (output.isTrashcanOutput)
                return output;
        }
        return null;
    }

    //a, b, c

    //A: a, b, c
    //B: d, e, f
    //C: g, h, i

    public List<BuildingIOManager> GetInputConveyorGroup()
    {
        List<BuildingIOManager> list = new List<BuildingIOManager>();

        foreach(BuildingIO io in inputs)
        {
            if (io.attachedIO)
            {
                list.Add(io.attachedIO.myManager);
                list.AddRange(io.attachedIO.myManager.GetInputConveyorGroup());
            }
        }


        return list;
    }

    public BuildingIOManager[] GetManagers(int inputID) 
    {
        List<BuildingIOManager> toReturn = new List<BuildingIOManager>();

        if (!isConveyor || inputs.Length <= inputID || !inputs[inputID].attachedIO)
            return null;

        BuildingIOManager next = inputs[inputID].attachedIO.myManager;

        for (; ;)
        {
            if (next.isConveyor)
            {
                foreach (BuildingIO io in next.inputs) 
                {
                    if (io.attachedIO.myManager.isConveyor)
                    {
                        next = io.myManager;
                        toReturn.Add(next);
                    }
                }
            }
            else
                break;
        }

        Debug.Log("Found " + toReturn.Count);
        return toReturn.ToArray();
    }
}
