﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum APMStatus { Idle, Blocked, Crafting }

public class CraftingData
{
    public MachineRecipe CurrentRecipe;
    public int inputID;
    public int outputID;

    private Dictionary<MachineRecipe.InputData, int> inputData;
    public Dictionary<MachineRecipe.InputData, int> InputData
    {
        get
        {
            return inputData;
        }
        set
        {
            inputData = new Dictionary<MachineRecipe.InputData, int>(value);
        }
    }

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
    public RecipeFilter recipeFilter;
    public List<MachineRecipe> allowedRecipes = new List<MachineRecipe>();
    private MachineRecipe currentRecipe;
    public float baseTimeMultiplier = 1;
    public int inputSpace;
    private bool isInputFull;
    public int outputSpace;
    private bool isOutputFull;

    private APMStatus currentStatus;

    // Key is the needed recipe item
    // Value is inputID
    [HideInInspector]
    public Dictionary<MachineRecipe.InputData, int> inputData = new Dictionary<MachineRecipe.InputData, int>();

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
            InitIOData();

            Debug.Log("Set recipe: " + value?.name, this);

            if (!value)
                CurrentStatus = APMStatus.Blocked;
            else
                CurrentStatus = APMStatus.Idle;
        }
    }

    public APMStatus CurrentStatus
    {
        get => currentStatus;
        set
        {
            /*if (value == APMStatus.Crafting)
                mc.BuildingIOManager.SetConveyorGroupState(WorkStateEnum.Off);
                
            else
                mc.BuildingIOManager.SetConveyorGroupState(WorkStateEnum.On);*/

            currentStatus = value;
        }
    }

    public void Init()
    {
        mc.BuildingIOManager.OnItemEnterInput.AddListener(OnItemEnterInput);

        if (CurrentRecipe)
            InitIOData();
        else
            CurrentStatus = APMStatus.Blocked;

        allowedRecipes = RecipeManager.GetRecipes(recipeFilter).allowed;

        foreach (BuildingIO io in mc.BuildingIOManager.outputs)
        {
            io.outputMaxQueueSize = outputSpace;
        }
    }

    private void InitIOData()
    {
        inputData.Clear();
        outputData.Clear();

        if (CurrentRecipe)
        {
            int buildingInputs = mc.BuildingIOManager.inputs.Length;
            foreach (MachineRecipe.InputsData data in CurrentRecipe.inputs)
            {
                for (int i = 0; i < data.inputs.Length; i++)
                {
                    int inputIDToApply = data.inputs[i].inputID + 1;
                    if (data.inputs[i].inputID == -1)
                    {
                        inputIDToApply = -1;
                    }
                    else
                    {
                        if (inputIDToApply >= buildingInputs)
                            inputIDToApply = buildingInputs;
                    }

                    inputData.Add(data.inputs[i], inputIDToApply);
                }
            }

            int buildingOutputs = mc.BuildingIOManager.outputs.Length;
            foreach (MachineRecipe.OutputsData data in CurrentRecipe.outputs)
            {
                for (int i = 0; i < data.outputs.Length; i++)
                {
                    int outputIDToApply = data.outputs[i].outputID + 1;
                    if (outputIDToApply >= buildingOutputs)
                        outputIDToApply = buildingOutputs;

                    outputData.Add(data.outputs[i], outputIDToApply);
                }
            }
        }
    }

    public void OnInputButtonPressed(InputSelector caller)
    {
        int newInputID = caller.InputID + 1;

        if (caller.InputID == -1)
            return;
        else if (newInputID <= mc.BuildingIOManager.inputs.Length)
            inputData[caller.value] = newInputID;
        else
        {
            newInputID = 1;
            inputData[caller.value] = newInputID;
        }

        caller.InputID = newInputID;
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
        if (IsAllowedToEnter(ItemEnterInfo))
            AcceptItemInside(ItemEnterInfo);
        else
            return;

        (bool, int, int) recipeInputInfo = IsAllowedToStartCrafting(ItemEnterInfo);

        if (recipeInputInfo.Item1)
            StartCrafting(recipeInputInfo.Item2, recipeInputInfo.Item3);
    }

    public bool IsAllowedToEnter(OnItemEnterEvent ItemEnterInfo)
    {
        if (!CurrentRecipe) // check if we have any recipe to work with
        {
            ItemLog(ItemEnterInfo.item.name, "Item attempts to enter but there's no recipe!!!", this);
            return false;
        }

        MachineRecipe.InputsData recipeData = CurrentRecipe.inputs.Find(data =>
        {
            foreach (MachineRecipe.InputData inputData in data.inputs)
                if (inputData.item.ID == ItemEnterInfo.item.ID) return true;

            return false;
        });

        if (recipeData == null)
        {
            mc.Building.SetIndicator(BuildingManager.instance.ErrorIndicator);
            return false;
        }

        bool allow = false;
        foreach (MachineRecipe.InputData data in recipeData.inputs)
        {
            // this if is probably not needed
            if (Equals(recipeData, default)) // the item that attempts to enter is not expected to enter
            {
                //ItemLog(ItemEnterInfo.item.name, "This item was not expected to enter this building!", this);
                continue;
            }

            // check if the input where the item attempts to enter is the correct one
            if (data.inputID != -1 && data.inputID != ItemEnterInfo.inputID)
            {
                //Debug.LogWarning("This item was not expected to enter this input", this);
                continue;
            }

            allow = true;
            break;
        }

        if (!allow)
        {
            mc.Building.SetIndicator(BuildingManager.instance.ErrorIndicator);
            return false;
        }


        // storage check here
        if (!IsStorageSufficient(ItemEnterInfo))
            return false;

        return true;
    }

    private (bool, int, int) IsAllowedToStartCrafting(OnItemEnterEvent ItemEnterInfo)
    {
        bool failed = false;
        int inputID = 0;
        int outputID = 0;


        // check if the outputs' queues have enough space to fit the output items
        foreach (KeyValuePair<MachineRecipe.OutputData, int> kvp in outputData)
        {
            BuildingIO io = mc.BuildingIOManager.outputs[kvp.Value - 1];
            if (io.itemsToSpawn.Count + kvp.Key.amount > io.outputMaxQueueSize)
            {
                ItemLog(ItemEnterInfo.item.name, "Not enough space to one or more of the output/s", this);
                mc.Building.SetIndicator(BuildingManager.instance.ErrorIndicator);
                return (false, inputID, outputID);
            }
        }

        mc.Building.RemoveIndicator();

        foreach (MachineRecipe.InputsData inputsData in CurrentRecipe.inputs)
        {
            foreach (MachineRecipe.InputData data in inputsData.inputs)
            {
                ItemData itemToCheck = data.item;

                // check if we have the enough quantity of it available to start crafting
                if (!mc.BuildingIOManager.itemsInside.ContainsKey(itemToCheck))
                {
                    //A required item type is missing from itemsInside!
                    failed = true;
                    break; // stop and go to the new InputsData
                }

                Debug.Log("found one similar item");
                if (mc.BuildingIOManager.itemsInside[itemToCheck] < data.amount)
                {
                    //Still, not all items are present inside
                    failed = true;
                    break; // stop and go to the new InputsData
                }
                Debug.Log("found enough quantity!");

                // ok we have all needed items present

                // locate the inputID and the outputID of that recipe
                inputID = CurrentRecipe.inputs.FindIndex(inputData => inputData == inputsData);
                outputID = inputsData.outputListID;
            }

            if (failed)
            {
                Debug.Log("A condition failed so the APM can't start crafting...");
                break;
            }
                
        }

        return (!failed, inputID, outputID);
    }

    private bool IsStorageSufficient(OnItemEnterEvent ItemEnterInfo)
    {
        // If the items that attempts to enter has a quantity larger that the allowed
        if (mc.BuildingIOManager.itemsInside.FirstOrDefault(kvp => kvp.Key == ItemEnterInfo.item).Value == inputSpace)
        {
            ItemLog(ItemEnterInfo.item.name, "There's not enough space for this item!", this);
            mc.Building.SetIndicator(BuildingManager.instance.ErrorIndicator);
            return false;
        }

        mc.Building.RemoveIndicator();
        return true;
    }

    #region Crafting procedure

    private void StartCrafting(int inputID, int outputID)
    {
        Debug.Log("Start crafting!", this);
        CurrentStatus = APMStatus.Crafting;

        CraftingData data = new CraftingData()
        {
            inputID = inputID,
            outputID = outputID,
            InputData = inputData,
            OutputData = outputData,
            CurrentRecipe = currentRecipe
        };

        // look which InputsData is filled
        // get and save corresponding OutputData

        if (CurrentlyCrafting.Count != 0)
            CurrentlyCrafting.Dequeue();

        CurrentlyCrafting.Enqueue(data);

        foreach (MachineRecipe.InputData toRemove in data.CurrentRecipe.inputs[inputID].inputs)
        {
            mc.BuildingIOManager.itemsInside.Remove(toRemove.item);
        }

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
        for (int i = 0; i < currentlyCrafting.CurrentRecipe.outputs[currentlyCrafting.outputID].outputs.Length; i++)
        {
            KeyValuePair<MachineRecipe.OutputData, int> entry = currentlyCrafting.OutputData.ElementAt(i);

            for (int t = 0; t < entry.Key.amount; t++)
            {
                mc.BuildingIOManager.outputs[entry.Value - 1].AddToSpawnQueue(entry.Key.item);
            }

            mc.Building.RemoveIndicator();
            CurrentStatus = APMStatus.Idle;
        }
    }

    private void AcceptItemInside(OnItemEnterEvent ItemEnterInfo)
    {
        if (ItemEnterInfo.sceneInstance)
        {
            ObjectPoolManager.instance.DestroyObject(ItemEnterInfo.sceneInstance);
            mc.BuildingIOManager.itemsInside = ItemEnterInfo.proposedItems;
        }
    }

    #endregion

    private void ItemLog(string itemName, string message, UnityEngine.Object highlight = null)
    {
        Debug.Log($"[Recipe: {CurrentRecipe.name}] [Item: {itemName}] {message}", highlight);
    }
}
