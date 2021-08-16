//
// Developed by TerraStudios (https://github.com/TerraStudios)
//
// Copyright(c) 2020-2021 Konstantin Milev (konstantin890 | milev109@gmail.com)
// Copyright(c) 2020-2021 Yerti (UZ9)
//
// The following script has been written by either konstantin890 or Yerti (UZ9) or both.
// This file is covered by the GNU GPL v3 license. Read LICENSE.md for more information.
// Past NDA/MNDA and Confidential notices are revoked and invalid since no agreement took place. Read README.md for more information.
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
