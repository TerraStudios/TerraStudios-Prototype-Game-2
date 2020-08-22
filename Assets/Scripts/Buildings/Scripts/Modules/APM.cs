using System;
using System.Collections;
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

            Debug.Log("Set recipe: " + value?.name);

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

    private void OnItemEnterInput(OnItemEnterEvent itemEnterInfo)
    {
        if (!CurrentRecipe) // check if we have any recipe to work with
        {
            Debug.LogError("Item attempts to enter but there's no recipe!!!");
            return;
        }

        if (CurrentStatus == APMStatus.Crafting) // check if the APM is currently crafting
        {
            Debug.Log("There's currently crafting ongoing!");
            return;
        }

        Dictionary<int, HashSet<MachineRecipe.ItemTagData>> inputData = CurrentRecipe.GetInputData();

        HashSet<MachineRecipe.ItemTagData> recipeInput = inputData.GetOrDefault(itemEnterInfo.inputID, null); // Returns null if it can't find the item

        var info = RetrieveItemInfo(inputData, itemEnterInfo.item);

        if (info == null) // Couldn't find any ItemTags associated with the item (Is this item expected to enter this input?)
        {
            Debug.LogWarning("Rejecting item: couldn't find an ItemTag associated with the item.");
            return;
        }

        //var infoValue = info.Value;

        for (int i = 0; i < outputData.Keys.Count; i++)
        {
            BuildingIO io = mc.BuildingIOManager.outputs[i];
            if (io.itemsToSpawn.Count + outputData.Keys.ElementAt(i).amount > io.outputMaxQueueSize)
            {
                Debug.LogWarning("Not enough space to one or more of the output/s");
                return;
            }
        }




        Debug.LogWarning("Accepting item..");
        AcceptItemInside(itemEnterInfo);
        Debug.LogWarning("Finished accepting item");


        // Loops through each recipe input and checks if 
        foreach (var itemData in inputData)
        {
            //Loop through each input tag, and for each item that belongs check its count
            foreach (var tagData in itemData.Value)
            {

                bool foundItem = false;
                //Loop through each item inside and check if the tag fits it 
                foreach (var itemInside in mc.BuildingIOManager.itemsInside)
                {
                    if (tagData.tag.Matches(itemInside.Key))
                    {
                        foundItem = true;
                        //Tag matches, check if it doesn't have the correct amount
                        if (itemInside.Value < tagData.amount)
                        {
                            foundItem = false;
                        }
                    }

                }

                if (!foundItem)
                {
                    Debug.LogWarning("Couldn't find a material for crafting");
                    return;
                }

            }
        }

        Debug.LogWarning("Beginning crafting...");
        StartCrafting(itemEnterInfo); // Initiates crafting process
    }

    #region Crafting procedure

    private void StartCrafting(OnItemEnterEvent ItemEnterInfo)
    {
        Debug.Log("Start crafting!");
        AcceptItemInside(ItemEnterInfo);
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
            Destroy(ItemEnterInfo.sceneInstance);
    }

    private KeyValuePair<MachineRecipe.ItemTagData, int>? RetrieveItemInfo(Dictionary<int, HashSet<MachineRecipe.ItemTagData>> recipeInput, ItemData item)
    {
        foreach (var input in recipeInput)
        {
            // Loops through each recipe input and checks if 
            foreach (var itemData in input.Value)
            {
                if (itemData.tag.Matches(item)) return new KeyValuePair<MachineRecipe.ItemTagData, int>(itemData, input.Key);
            }
        }

        return null;
    }

    #endregion
}
