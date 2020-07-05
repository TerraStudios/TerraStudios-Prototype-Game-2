using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipePreset : ScriptableObject
{
    public MachineRecipe[] allowedRecipes;
    public MachineRecipe[] blockedRecipes;
}
