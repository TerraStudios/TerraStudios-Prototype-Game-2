using UnityEngine;
using System.Collections.Generic;

public class RecipeManager
{

    private MachineRecipe[] recipes;
    private ItemCategory[] categories;

    private static RecipeManager _instance;

    public static RecipeManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RecipeManager();
            }

            return _instance;
        }
    }

    public RecipeManager()
    {
        this.categories = Resources.LoadAll<ItemCategory>("Categories");
        this.recipes = Resources.LoadAll<MachineRecipe>("Recipes");
    }

    public MachineRecipe[] RetrieveRecipes()
    {
        return recipes;
    }

    public ItemCategory[] RetrieveCategories()
    {
        return categories;
    }






}
