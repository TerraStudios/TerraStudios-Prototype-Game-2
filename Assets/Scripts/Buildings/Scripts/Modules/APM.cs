using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APM : MonoBehaviour
{
    public ModuleConnector mc;
    public bool allowAllRecipes;
    public RecipePreset recipePreset;
    public MachineRecipe currentRecipe;

    public void Init()
    {
        mc.BuildingIOManager.OnItemEnterInput.AddListener(OnItemEnterInput);
    }

    private void OnItemEnterInput(OnItemEnterEvent args)
    {
        if (!currentRecipe)
        {
            Debug.LogWarning("Item attempts to enter but there's no recipe!!!");
            return;
        }

        if (!Array.Exists(currentRecipe.inputs, inputData => inputData.item == args.item)) // check if the item entering is expected to enter
        {
            Debug.LogWarning("This item was not expected to enter this building!");
            return;
        }

        foreach (MachineRecipe.InputData required in currentRecipe.inputs) // check if we have all required items
        {
            if (required.item is ItemData)
            {
                ItemData itemToCheck = required.item as ItemData;
                if (itemToCheck.ID != args.item.ID)
                {
                    Debug.LogWarning("Still, not all items are present inside");
                    return;
                }
            }
            else if (required.item is ItemCategory)
            {
                ItemCategory cat = required.item as ItemCategory;
                if (cat != args.item.ItemCategory)
                {
                    Debug.LogWarning("Still, not all items are present inside");
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
        mc.BuildingIOManager.itemsInside = new List<ItemData>(); // remove all items inside

        StartCoroutine(RunCraftingTimer());
    }

    IEnumerator RunCraftingTimer()
    {
        yield return new WaitForSeconds(currentRecipe.baseTime);
        ExecuteCrafting();
    }

    private void ExecuteCrafting()
    {
        for (int i = 0; i < currentRecipe.outputs.Length; i++)
        {
            mc.BuildingIOManager.outputs[i].SpawnItemObj(currentRecipe.outputs[i].item);
        }

        Debug.Log("Finished crafting!");
    }

    #endregion
}
