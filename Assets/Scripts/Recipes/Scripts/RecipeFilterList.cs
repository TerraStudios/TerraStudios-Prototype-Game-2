using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Recipe Filter List", menuName = "Recipe/Recipe Filter List")]
public class RecipeFilterList : ScriptableObject
{
    public List<MachineRecipe> recipes;
}
