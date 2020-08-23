﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum APMStatus { Idle, Blocked, Crafting }

public class CraftingData
{
    public MachineRecipe CurrentRecipe;
    private Dictionary<MachineRecipe.OutputData, int> outputData;

    public Dictionary<MachineRecipe.OutputData, int> OutputData
    {
        get
        {
            return outputData;
        }
        set
        {
            outputData = new Dictionary<MachineRecipe.OutputData, int>(value);
        }
    }
}

public class APM : MonoBehaviour
{
    public ModuleConnector mc;
    public RecipeFilter recipePreset;
    private MachineRecipe currentRecipe;
    public float baseTimeMultiplier = 1;
    public APMStatus CurrentStatus;

    // Key is the needed recipe item
    // Value is outputID
    [HideInInspector]
    public Dictionary<MachineRecipe.OutputData, int> outputData = new Dictionary<MachineRecipe.OutputData, int>();

    public Queue<CraftingData> CurrentlyCrafting = new Queue<CraftingData>();


    public MachineRecipe CurrentRecipe
    {
        get => currentRecipe;
        set
        {
            if (CurrentRecipe == value)
                return;

            currentRecipe = value;
            InitOutputData();

            Debug.Log("Set recipe: " + value?.name, this);

            if (!value)
                CurrentStatus = APMStatus.Blocked;
            else
                CurrentStatus = APMStatus.Idle;
        }
    }

    public void Init()
    {
        mc.BuildingIOManager.OnItemEnterInput.AddListener(OnItemEnterInput);

        if (CurrentRecipe)
            InitOutputData();
        else
            CurrentStatus = APMStatus.Blocked;
    }

    private void InitOutputData()
    {
        outputData.Clear();

        if (CurrentRecipe)
        {
            int buildingOutputs = mc.BuildingIOManager.outputs.Length;
            for (int i = 0; i < CurrentRecipe.outputs.Length; i++)
            {
                int outputIDToApply = i + 1;
                if (outputIDToApply >= buildingOutputs)
                    outputIDToApply = buildingOutputs;

                outputData.Add(CurrentRecipe.outputs[i], outputIDToApply);
            }
        }
    }

    public void OnOutputButtonPressed(OutputSelector caller)
    {
        int newOutputID = caller.OutputID + 1;

        if (newOutputID <= mc.BuildingIOManager.outputs.Length)
            outputData[caller.value] = newOutputID;
        else
        {
            newOutputID = 1;
            outputData[caller.value] = newOutputID;
        }

        caller.OutputID = newOutputID;
    }

    private void OnItemEnterInput(OnItemEnterEvent ItemEnterInfo)
    {
        if (!CurrentRecipe) // check if we have any recipe to work with
        {
            Debug.LogError("Item attempts to enter but there's no recipe!!!", this);
            return;
        }

        MachineRecipe.InputData recipeData = CurrentRecipe.inputs.FirstOrDefault(data =>
        {
            if (data.item is ItemData)
            {
                if ((data.item as ItemData).ID == ItemEnterInfo.item.ID) return true;
            }
            else
            {
                if ((data.item as ItemCategory) == ItemEnterInfo.item.ItemCategory) return true;
            }

            return false;
        });

        if (Equals(recipeData, default))
        {
            Debug.LogError("This item was not expected to enter this building!", this);
            return;
        }

        if (recipeData.inputID != -1)
        {
            if (recipeData.inputID != ItemEnterInfo.inputID)
            {
                Debug.LogWarning("This item was not expected to enter this input", this);
                return;
            }
        }

        if (recipeData.item is ItemData)
        {
            ItemData itemToCheck = recipeData.item as ItemData;

            if (ItemEnterInfo.proposedItems[itemToCheck] > recipeData.amount) // check if we're full of that item
            {
                Debug.LogWarning("We're already full of this item!", this);
                return;
            }
        }
        else if (recipeData.item is ItemCategory)
        {
            ItemCategory cat = recipeData.item as ItemCategory;

            if (ItemEnterInfo.proposedItems.FirstOrDefault(kvp => kvp.Key.ItemCategory == cat).Value > recipeData.amount)
            {
                Debug.LogWarning("We're already full of this item!", this); // check if we're full of that item
                return;
            }
        }

        // check if the outputs' queues have enough space to fit the output items
        foreach (KeyValuePair<MachineRecipe.OutputData, int> kvp in outputData)
        {
            BuildingIO io = mc.BuildingIOManager.outputs[kvp.Value - 1];
            if (io.itemsToSpawn.Count + kvp.Key.amount > io.outputMaxQueueSize)
            {
                Debug.LogWarning("Not enough space to one or more of the output/s", this);
                return;
            }
        }

        AcceptItemInside(ItemEnterInfo);

        // Check if should start crafting

        foreach (MachineRecipe.InputData inputData in CurrentRecipe.inputs)
        {
            if (inputData.item is ItemData)
            {
                ItemData itemToCheck = inputData.item as ItemData;

                // check if we have the enough quantity of it available to start crafting
                if (!mc.BuildingIOManager.itemsInside.ContainsKey(itemToCheck))
                {
                    Debug.Log("A required item type is missing from itemsInside!", this);
                    return;
                }
                else
                {
                    if (mc.BuildingIOManager.itemsInside[itemToCheck] < inputData.amount)
                    {
                        Debug.Log("Still, not all items are present inside", this);
                        return;
                    }
                }
            }
            else if (inputData.item is ItemCategory)
            {
                ItemCategory cat = inputData.item as ItemCategory;

                // check if we have the enough quantity of it available to start crafting
                if (mc.BuildingIOManager.itemsInside.FirstOrDefault(kvp => kvp.Key.ItemCategory == cat).Value < inputData.amount)
                {
                    Debug.Log("Still, not all items are present inside", this);
                    return;
                }
            }
        }

        StartCrafting(); // ready to go
    }

    #region Crafting procedure

    private void StartCrafting()
    {
        Debug.Log("Start crafting!", this);
        CurrentStatus = APMStatus.Crafting;

        CraftingData data = new CraftingData()
        {
            OutputData = outputData,
            CurrentRecipe = currentRecipe
        };

        if (CurrentlyCrafting.Count != 0)
            CurrentlyCrafting.Dequeue();

        CurrentlyCrafting.Enqueue(data);

        mc.BuildingIOManager.itemsInside.Clear(); // remove all items inside

        StartCoroutine(RunCraftingTimer());
    }

    IEnumerator RunCraftingTimer()
    {
        yield return new WaitForSeconds(CurrentlyCrafting.Peek().CurrentRecipe.baseTime * baseTimeMultiplier);
        ExecuteCrafting();
    }

    private void ExecuteCrafting()
    {
        CraftingData currentlyCrafting = CurrentlyCrafting.Peek();
        for (int i = 0; i < currentlyCrafting.OutputData.Count; i++)
        {
            KeyValuePair<MachineRecipe.OutputData, int> entry = currentlyCrafting.OutputData.ElementAt(i);

            for (int t = 0; t < entry.Key.amount; t++)
            {
                mc.BuildingIOManager.outputs[entry.Value - 1].AddToSpawnQueue(entry.Key.item);
            }

            CurrentStatus = APMStatus.Idle;
        }
    }

    private void AcceptItemInside(OnItemEnterEvent ItemEnterInfo)
    {
        if (ItemEnterInfo.sceneInstance)
        {
            Destroy(ItemEnterInfo.sceneInstance);
            mc.BuildingIOManager.itemsInside = ItemEnterInfo.proposedItems;
        }
    }

    #endregion
}
