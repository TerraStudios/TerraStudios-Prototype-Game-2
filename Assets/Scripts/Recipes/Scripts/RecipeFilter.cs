using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe Preset", menuName = "Recipe Preset")]
public class RecipeFilter : ScriptableObject
{
    [SerializeField] private List<MachineRecipe> allowedRecipes;
    public List<MachineRecipe> blockedRecipes;

    public List<MachineRecipe> AllowedRecipes
    {
        get
        {
            if (allowedRecipes.Count() != 0)
                return allowedRecipes;
            else
            {
                List<MachineRecipe> toReturn = new List<MachineRecipe>();
                foreach (MachineRecipe recipe in RecipeManager.instance.RetrieveRecipes())
                {
                    if (!blockedRecipes.Contains(recipe))
                        toReturn.Add(recipe);
                }
                return toReturn;
            }
        }
        set => allowedRecipes = value; 
    }
}
