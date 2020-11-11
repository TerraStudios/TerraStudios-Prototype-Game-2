using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RecipeManager : MonoBehaviour
{
    private static List<MachineRecipe> recipes;
    private static ItemCategory[] categories;

    public static RecipeManager instance;

    private void Awake()
    {
        instance = this;
        LoadResources();
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

    public static void LoadResources() 
    {
        categories = Resources.LoadAll<ItemCategory>("");
        recipes = Resources.LoadAll<MachineRecipe>("").ToList();
    }

    public static (List<MachineRecipe> allowed, List<MachineRecipe> blocked) GetRecipes(RecipeFilter filter)
    {
        List<MachineRecipe> allowedRecipes = new List<MachineRecipe>();
        List<MachineRecipe> blockedRecipes = new List<MachineRecipe>();

        // Get recipes from the automatic fields
        if (filter.enableAutomaticList)
        {
            foreach (MachineRecipe recipe in recipes)
            {
                bool fits = false;
                foreach (MachineRecipe.InputData data in recipe.inputs)
                {
                    if (data.inputID < filter.inputsAmount)
                        fits = true;
                    else
                    {
                        fits = false;
                        break;
                    }
                }

                if (fits)
                {
                    if (filter.type == RecipeType.Allowed)
                    {
                        allowedRecipes.Add(recipe);
                    }
                    else if (filter.type == RecipeType.Blocked)
                    {
                        blockedRecipes.Add(recipe);
                    }
                }
                else
                    continue;
            }
        }

        // Get recipes from the manual fields
        foreach (ManualRecipeList mrl in filter.manualList)
        {
            foreach (MachineRecipe recipe in mrl.list.recipes)
            {
                // if allowed recipe appears in blocked -> allow it
                // if allowed recipe doesn't appear in allowed recipes -> add it
                if (mrl.type == RecipeType.Allowed)
                {
                    if (blockedRecipes.Contains(recipe))
                    {
                        blockedRecipes.Remove(recipe);
                        allowedRecipes.Add(recipe);
                    }
                    if (!allowedRecipes.Contains(recipe))
                        allowedRecipes.Add(recipe);
                }

                // if blocked recipe appears in allowed -> block it
                // if blocked recipe doesn't appear in blocked recipes -> add it
                if (mrl.type == RecipeType.Blocked)
                {
                    if (allowedRecipes.Contains(recipe))
                    {
                        allowedRecipes.Remove(recipe);
                        blockedRecipes.Add(recipe);
                    }
                    if (!blockedRecipes.Contains(recipe))
                        blockedRecipes.Add(recipe);
                }
            }
        }

        return (allowedRecipes, blockedRecipes);
    }
}
