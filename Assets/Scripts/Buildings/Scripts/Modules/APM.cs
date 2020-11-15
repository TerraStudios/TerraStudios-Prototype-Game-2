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
    public RecipeFilter recipeFilter;
    public List<MachineRecipe> allowedRecipes = new List<MachineRecipe>();
    private MachineRecipe currentRecipe;
    public float baseTimeMultiplier = 1;
    private APMStatus currentStatus;

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

    public APMStatus CurrentStatus 
    {
        get => currentStatus;
        set
        {
            if (value == APMStatus.Crafting)
                mc.BuildingIOManager.SetConveyorGroupState(WorkStateEnum.Off);
                
            else
                mc.BuildingIOManager.SetConveyorGroupState(WorkStateEnum.On);

            currentStatus = value;
        }
    }

    public void Init()
    {
        mc.BuildingIOManager.OnItemEnterInput.AddListener(OnItemEnterInput);

        if (CurrentRecipe)
            InitOutputData();
        else
            CurrentStatus = APMStatus.Blocked;

        allowedRecipes = RecipeManager.GetRecipes(recipeFilter).allowed;
    }

    private void InitOutputData()
    {
        outputData.Clear();

        if (CurrentRecipe)
        {
            int buildingOutputs = mc.BuildingIOManager.outputs.Length;
            foreach (MachineRecipe.OutputData[] data in CurrentRecipe.outputs)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    int outputIDToApply = i + 1;
                    if (outputIDToApply >= buildingOutputs)
                        outputIDToApply = buildingOutputs;

                    outputData.Add(data[i], outputIDToApply);
                }
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
        if (IsAllowedToEnter(ItemEnterInfo))
            AcceptItemInside(ItemEnterInfo);

        if (IsAllowedToStartCrafting(ItemEnterInfo))
            StartCrafting();
    }

    public bool IsAllowedToEnter(OnItemEnterEvent ItemEnterInfo)
    {
        if (!CurrentRecipe) // check if we have any recipe to work with
        {
            ItemLog(ItemEnterInfo.item.name, "Item attempts to enter but there's no recipe!!!", this);
            return false;
        }

        MachineRecipe.InputData[] recipeData = CurrentRecipe.inputs.FirstOrDefault(data =>
        {
            foreach (MachineRecipe.InputData inputData in data)
                if ((inputData.item).ID == ItemEnterInfo.item.ID) return true;

            return false;
        });

        foreach (MachineRecipe.InputData data in recipeData)
        {
            if (Equals(recipeData, default))
            {
                ItemLog(ItemEnterInfo.item.name, "This item was not expected to enter this building!", this);
                return false;
            }

            if (data.inputID != -1)
            {
                if (data.inputID != ItemEnterInfo.inputID)
                {
                    Debug.LogWarning("This item was not expected to enter this input", this);
                    return false;
                }
            }
        }

        /*if (recipeData.item is ItemData)
        {
            ItemData itemToCheck = recipeData.item as ItemData;

            if (ItemEnterInfo.proposedItems[itemToCheck] > recipeData.amount) // check if we're full of that item
            {
                ItemLog(ItemEnterInfo.item.name, "We're already full of this item!", this);
                return false;
            }
        }*/

        return true;
    }

    private bool IsAllowedToStartCrafting(OnItemEnterEvent ItemEnterInfo)
    {
        // check if the outputs' queues have enough space to fit the output items
        foreach (KeyValuePair<MachineRecipe.OutputData, int> kvp in outputData)
        {
            BuildingIO io = mc.BuildingIOManager.outputs[kvp.Value - 1];
            if (io.itemsToSpawn.Count + kvp.Key.amount > io.outputMaxQueueSize)
            {
                ItemLog(ItemEnterInfo.item.name, "Not enough space to one or more of the output/s", this);
                return false;
            }
        }

        foreach (MachineRecipe.InputData[] inputData in CurrentRecipe.inputs)
        {
            foreach (MachineRecipe.InputData data in inputData)
            {
                ItemData itemToCheck = data.item;

                // check if we have the enough quantity of it available to start crafting
                if (!mc.BuildingIOManager.itemsInside.ContainsKey(itemToCheck))
                {
                    ItemLog(ItemEnterInfo.item.name, "A required item type is missing from itemsInside!", this);
                    return false;
                }
                else
                {
                    if (mc.BuildingIOManager.itemsInside[itemToCheck] < data.amount)
                    {
                        ItemLog(ItemEnterInfo.item.name, "Still, not all items are present inside", this);
                        return false;
                    }
                }
            }
        }

        return true;
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
            ObjectPoolManager.instance.DestroyObject(ItemEnterInfo.sceneInstance);
            mc.BuildingIOManager.itemsInside = ItemEnterInfo.proposedItems;
        }
    }

    #endregion

    private void ItemLog(string itemName, string message, Object highlight = null)
    {
        Debug.Log($"[Recipe: {CurrentRecipe.name}] [Item: {itemName}] {message}", highlight);
    }
}
