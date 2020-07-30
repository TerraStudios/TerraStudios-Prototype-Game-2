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
    public MachineRecipe currentRecipe;

    // Key is the needed recipe item
    // Value is outputID
    [HideInInspector]
    public Dictionary<MachineRecipe.OutputData, int> outputData = new Dictionary<MachineRecipe.OutputData, int>();

    public void Init()
    {
        mc.BuildingIOManager.OnItemEnterInput.AddListener(OnItemEnterInput);

        InitOutputData();
    }

    private void InitOutputData() 
    {
        for (int i = 0; i < currentRecipe.outputs.Length; i++)
        {
            outputData.Add(currentRecipe.outputs[i], i + 1);
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
        if (!currentRecipe) // check if we have any recipe to work with
        {
            Debug.LogError("Item attempts to enter but there's no recipe!!!");
            return;
        }

        foreach (MachineRecipe.InputData recipeData in currentRecipe.inputs) // check if we have all required items
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
                    return;
                }
            }
        }

        StartCrafting(); // ready to go
    }

    #region Crafting procedure

    private void StartCrafting()
    {
        Debug.Log("Start crafting!");
        mc.BuildingIOManager.itemsInside.Clear(); // remove all items inside

        StartCoroutine(RunCraftingTimer());
    }

    IEnumerator RunCraftingTimer()
    {
        yield return new WaitForSeconds(currentRecipe.baseTime);
        ExecuteCrafting();
    }

    private void ExecuteCrafting()
    {
        for (int i = 0; i < outputData.Count; i++)
        {
            KeyValuePair<MachineRecipe.OutputData, int> entry = outputData.ElementAt(i);

            mc.BuildingIOManager.outputs[entry.Value - 1].SpawnItemObj(entry.Key.item);
        }

        Debug.Log("Finished crafting!");
    }

    #endregion
}
