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

    public void Init()
    {
        mc.BuildingIOManager.OnItemEnterInput.AddListener(OnItemEnterInput);
    }

    private void OnItemEnterInput(OnItemEnterEvent ItemEnterInfo)
    {
        if (!currentRecipe) // check if we have any recipe to work with
        {
            Debug.LogWarning("Item attempts to enter but there's no recipe!!!");
            return;
        }

        foreach (MachineRecipe.InputData recipeData in currentRecipe.inputs) // check if we have all required items
        {
            if (recipeData.item is ItemData)
            {
                ItemData itemToCheck = recipeData.item as ItemData;
                // check if item entering is expected to enter
                if (itemToCheck.ID != ItemEnterInfo.item.ID)
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
                // check if item category entering is expected to enter
                if (cat != ItemEnterInfo.item.ItemCategory)
                {
                    Debug.LogWarning("This item was not expected to enter this building!");
                    return;
                }

                // check if we have the enough quantity of it available to start crafting
                if (mc.BuildingIOManager.itemsInside.Any(itemInsideData => itemInsideData.item.ItemCategory != cat))
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
        for (int i = 0; i < currentRecipe.outputs.Length; i++)
        {
            mc.BuildingIOManager.outputs[i].SpawnItemObj(currentRecipe.outputs[i].item);
        }

        Debug.Log("Finished crafting!");
    }

    #endregion
}
