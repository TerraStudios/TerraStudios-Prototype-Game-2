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
    /// <summary>
    /// Stores a list of recipes.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "New Recipe Filter List", menuName = "Recipe/Recipe Filter List")]
    public class RecipeFilterList : ScriptableObject
    {
        public List<MachineRecipe> recipes;
    }
}
