//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeManagement
{
    public enum RecipeType { Allowed, Blocked }

    /// <summary>
    /// Holds manual list properties for a RecipeFilter.
    /// </summary>
    [Serializable]
    public class ManualRecipeList
    {
        public RecipeType type;
        public RecipeFilterList list;
    }

    /// <summary>
    /// Conditions based on which recipes get retrieved.
    /// </summary>
    [CreateAssetMenu(fileName = "New Recipe Filter", menuName = "Recipe/Recipe Filter")]
    public class RecipeFilter : ScriptableObject
    {
        [Header("Automatic List")]
        public bool enableAutomaticList;
        public int buildingInputsAmount;
        public int buildingOutputsAmount;
        public RecipeType type;

        [Header("Manual List")]
        public List<ManualRecipeList> manualList = new List<ManualRecipeList>();
    }
}
