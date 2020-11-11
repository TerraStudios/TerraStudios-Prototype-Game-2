using System;
using System.Collections.Generic;
using UnityEngine;

public enum RecipeType { Allowed, Blocked }

[Serializable]
public class ManualRecipeList
{
    public RecipeType type;
    public RecipeFilterList list;
}

[CreateAssetMenu(fileName = "New Recipe Filter", menuName = "Recipe/Recipe Filter")]
public class RecipeFilter : ScriptableObject
{
    [Header("Automatic List")]
    public bool enableAutomaticList;
    public int buildingInputsAmount;
    public RecipeType type;

    [Header("Manual List")]
    public List<ManualRecipeList> manualList;
}
