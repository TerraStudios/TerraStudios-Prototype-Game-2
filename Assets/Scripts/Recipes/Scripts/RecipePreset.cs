using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe Preset", menuName = "Recipe Preset")]
public class RecipePreset : ScriptableObject
{
    [SerializeField] private MachineRecipe[] allowedRecipes;
    public MachineRecipe[] blockedRecipes;

    public MachineRecipe[] AllowedRecipes
    {
        get
        {
            if (allowedRecipes.Count() != 0)
                return allowedRecipes;
            else
                return RecipeManager.instance.RetrieveRecipes();
        }
        set => allowedRecipes = value; 
    }
}
