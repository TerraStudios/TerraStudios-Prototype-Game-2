using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RecipeManager : MonoBehaviour
{
    private List<MachineRecipe> recipes;
    private ItemCategory[] categories;

    public static RecipeManager instance;

    private void Awake()
    {
        instance = this;
        categories = Resources.LoadAll<ItemCategory>("");
        recipes = Resources.LoadAll<MachineRecipe>("").ToList();

        

        Debug.Log("Loaded " + recipes.Count() + " recipes and " + categories.Count() + " categories.");
    }

    public List<MachineRecipe> RetrieveRecipes()
    {
        return recipes;
    }

    public ItemCategory[] RetrieveCategories()
    {
        return categories;
    }

}
