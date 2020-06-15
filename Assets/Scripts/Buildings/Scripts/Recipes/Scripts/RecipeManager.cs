using UnityEngine;
using System.Collections.Generic;

public class RecipeManager
{
    public MachineRecipe[] grabRecipes()
    {
        return Resources.LoadAll<MachineRecipe>("Recipes");
    }

    public ItemCategory[] grabCategories()
    {
        return Resources.LoadAll<ItemCategory>("Categories");
    }






}
