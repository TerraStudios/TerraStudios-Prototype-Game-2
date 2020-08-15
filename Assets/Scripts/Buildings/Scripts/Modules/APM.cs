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

    private void OnItemEnterInput(OnItemEnterEvent ItemEnterInfo)
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

        // Check if should accept the item

        foreach (MachineRecipe.InputData recipeData in CurrentRecipe.inputs)
        {
            if (recipeData.inputID != -1)
            {
                Debug.LogWarning("InputID is: " + ItemEnterInfo.inputID);
                if (recipeData.inputID != ItemEnterInfo.inputID)
                {
                    continue;
                }
            }

            if (recipeData.item is ItemData)
            {
                ItemData itemToCheck = recipeData.item as ItemData;

                if (itemToCheck.ID != ItemEnterInfo.item.ID) // check if item entering is expected to enter
                {
                    Debug.LogWarning("This item was not expected to enter this building!");
                    return;
                }

                if (mc.BuildingIOManager.itemsInside.Any(itemInsideData => itemInsideData.quantity == recipeData.amount))
                {
                    Debug.LogWarning("We're already full of this item!");
                    return;
                }
            }
            else if (recipeData.item is ItemCategory)
            {
                ItemCategory cat = recipeData.item as ItemCategory;

                if (cat != ItemEnterInfo.item.ItemCategory) // check if item category entering is expected to enter
                {
                    Debug.LogWarning("This item was not expected to enter this building!");
                    return;
                }

                if (mc.BuildingIOManager.itemsInside.Any(itemInsideData => itemInsideData.item.ItemCategory == cat))
                {
                    Debug.LogWarning("We're already full of this item!");
                    return;
                }
            }            
        }

        AcceptItemInside(ItemEnterInfo);

        // Check if should start crafting

        foreach (MachineRecipe.InputData recipeData in CurrentRecipe.inputs)
        {
            if (recipeData.item is ItemData)
            {
                ItemData itemToCheck = recipeData.item as ItemData;

                // check if we have the enough quantity of it available to start crafting
                if (mc.BuildingIOManager.itemsInside.Any(itemInsideData => itemInsideData.quantity != recipeData.amount))
                {
                    Debug.LogWarning("Still, not all items are present inside");
                    return;
                }
            }
            else if (recipeData.item is ItemCategory)
            {
                ItemCategory cat = recipeData.item as ItemCategory;

                // check if we have the enough quantity of it available to start crafting
                if (mc.BuildingIOManager.itemsInside.Any(itemInsideData => itemInsideData.item.ItemCategory != cat))
                {
                    Debug.Log("Still, not all items are present inside");
                    return;
                }
            }
        }

        // check if the outputs' queues have enough space to fit the output items
        for (int i = 0; i < mc.BuildingIOManager.outputs.Length; i++)
        {
            BuildingIO io = mc.BuildingIOManager.outputs[i];
            if (io.itemsToSpawn.Count + outputData.Keys.ElementAt(i).amount > io.outputMaxQueueSize)
            {
                Debug.LogWarning("Not enough space to one or more of the output/s");
                return;
            }
        }

        StartCrafting(ItemEnterInfo); // ready to go
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

    #endregion
}
