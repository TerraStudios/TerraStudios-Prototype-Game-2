using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APM : MonoBehaviour
{
    public ModuleConnector mc;
    public bool allowAllRecipes;
    public RecipePreset recipePreset;
    public RecipePreset currentRecipe;

    private void Awake()
    {
        mc.BuildingIOManager.OnItemEnterInput.AddListener(OnItemEnterInput);
    }

    private void OnItemEnterInput(OnItemEnterArgs args) 
    {
        if (!currentRecipe)
        {
            Debug.LogWarning("Item attempts to enter but there's no recipe!!!");
            return;
        }


    }
}
