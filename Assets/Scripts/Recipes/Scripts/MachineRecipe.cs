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
using ItemManagement;
using UnityEngine;

namespace RecipeManagement
{
    /// <summary>
    /// Holds properties for APM Recipes.
    /// </summary>
    [CreateAssetMenu(fileName = "New Machine Recipe", menuName = "Recipe/Machine Recipe")]
    public class MachineRecipe : ScriptableObject
    {
        /// <summary>
        /// Represents a serializable dictionary containing the item and its amount 
        /// </summary>
        [Serializable]
        public class InputBatch
        {
            public int outputListId;
            public InputData[] inputs;
        }

        [Serializable]
        public class InputData
        {
            public ItemData item;
            public int amount;

            [Tooltip("Specifies the input ID from which the item should be expected to enter. -1 for undefined")]
            public int inputId = -1;
        }

        /// <summary>
        /// Represents a serializable dictionary containing the item and its amount.
        /// Uses ItemData instead of generic SO because it's not possible to output a category
        /// </summary>
        [Serializable]
        public class OutputBatch
        {
            public OutputData[] outputs;
        }

        [Serializable]
        public class OutputData
        {
            public ItemData item;
            public int amount;
            public int outputId;
        }

        [Header("Items")]

        [Tooltip("Specifies the input items and their corresponding amounts for the recipe.")]
        public List<InputBatch> inputs;

        [Tooltip("Specifies the output items and their corresponding amounts for the recipe.")]
        public List<OutputBatch> outputs;

        [Header("Other Info")]
        [Tooltip("The name of the recipe to display.")]
        public new string name;

        [Tooltip("The base time (in seconds) for the recipe to finish processing.")]
        public float baseTime;

        [Tooltip("The icon for this recipe.")]
        [NonSerialized] public Texture2D icon;

        public bool allowPlayerInputsConfiguration = true;
        public bool allowPlayerOutputsConfiguration = true;
    }
}
