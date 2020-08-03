using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class APM : MonoBehaviour
{
    public ModuleConnector mc;
    public bool allowAllRecipes;
    public RecipePreset recipePreset;
    private MachineRecipe currentRecipe;

    private bool isCrafting;

    // Key is the needed recipe item
    // Value is outputID
    [HideInInspector]
    public Dictionary<MachineRecipe.OutputData, int> outputData = new Dictionary<MachineRecipe.OutputData, int>();

    public bool IsCrafting 
    {
        get => isCrafting;
        set
        {
            isCrafting = value;
            if (value)
                BlockAllInputs();
            else
                UnblockAllInputs();
        }
    }

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
                BlockAllInputs();
            else
                UnblockAllInputs();
        }
    }

    public void Init()
    {
        mc.BuildingIOManager.OnItemEnterInput.AddListener(OnItemEnterInput);

        if (CurrentRecipe)
            InitOutputData();
        else
            BlockAllInputs();
    }

    private void InitOutputData() 
    {
        outputData.Clear();

        if (CurrentRecipe)
        {
            for (int i = 0; i < CurrentRecipe.outputs.Length; i++)
            {
                outputData.Add(CurrentRecipe.outputs[i], i + 1);
            }
        }
    }

    private void BlockAllInputs() 
    {
        foreach (BuildingIO input in mc.BuildingIOManager.inputs) // block all inputs so new items won't come in
        {
            input.BlockInput = true;
        }
    }

    private void UnblockAllInputs() 
    {
        foreach (BuildingIO input in mc.BuildingIOManager.inputs) // allow all inputs to accept new items and allow the pending item to go through
        {
            input.BlockInput = false;
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

        if (IsCrafting) // check if the APM is currently crafting
        {
            Debug.Log("There's currently crafting ongoing!");
            return;
        }

        foreach (MachineRecipe.InputData recipeData in CurrentRecipe.inputs) // check if we have all required items
        {
            if (recipeData.item is ItemData)
            {
                ItemData itemToCheck = recipeData.item as ItemData;
                
                if (itemToCheck.ID != ItemEnterInfo.item.ID) // check if item entering is expected to enter
                {
                    Debug.LogWarning("This item was not expected to enter this building!");
                    return;
                }

                // check if we have the enough quantity of it available to start crafting
                if (mc.BuildingIOManager.itemsInside.Any(itemInsideData => itemInsideData.quantity != recipeData.amount))
                {
                    Debug.LogWarning("Still, not all items are present inside");
                    AcceptItemInside(ItemEnterInfo);
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

                // check if we have the enough quantity of it available to start crafting
                if (mc.BuildingIOManager.itemsInside.Any(itemInsideData => itemInsideData.item.ItemCategory != cat))
                {
                    Debug.Log("Still, not all items are present inside");
                    AcceptItemInside(ItemEnterInfo);
                    return;
                }
            }

            if (recipeData.inputID != -1)
            {
                if (recipeData.inputID != ItemEnterInfo.inputID)
                {
                    Debug.LogWarning("This item was not expected to enter this input!");
                    return;
                }
            }
        }

        StartCrafting(ItemEnterInfo); // ready to go
    }

    #region Crafting procedure

    private void StartCrafting(OnItemEnterEvent ItemEnterInfo)
    {
        Debug.Log("Start crafting!");
        AcceptItemInside(ItemEnterInfo);
        IsCrafting = true;

        mc.BuildingIOManager.itemsInside.Clear(); // remove all items inside

        StartCoroutine(RunCraftingTimer());
    }

    IEnumerator RunCraftingTimer()
    {
        yield return new WaitForSeconds(CurrentRecipe.baseTime);
        ExecuteCrafting();
    }

    private void ExecuteCrafting()
    {
        for (int i = 0; i < outputData.Count; i++)
        {
            KeyValuePair<MachineRecipe.OutputData, int> entry = outputData.ElementAt(i);
            StartCoroutine(StartSpawning(entry));
        }

        IEnumerator StartSpawning(KeyValuePair<MachineRecipe.OutputData, int> entry)
        {
            for (int i = 0; i < entry.Key.amount; i++)
            {
                yield return new WaitForSeconds(1);
                ExecuteSpawning(entry);
            }
        }

        void ExecuteSpawning(KeyValuePair<MachineRecipe.OutputData, int> entry)
        {
            mc.BuildingIOManager.outputs[entry.Value - 1].SpawnItemObj(entry.Key.item);
        }

        IsCrafting = false;
        Debug.Log("Finished crafting!");
    }

    private void AcceptItemInside(OnItemEnterEvent ItemEnterInfo) 
    {
        if (ItemEnterInfo.sceneInstance)
            Destroy(ItemEnterInfo.sceneInstance);
    }

    #endregion
}
