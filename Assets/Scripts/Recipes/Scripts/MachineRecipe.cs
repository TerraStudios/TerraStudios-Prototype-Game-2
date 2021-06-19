//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
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
            public int outputListID;
            public InputData[] inputs;
        }

        [Serializable]
        public class InputData
        {
            public ItemData item;
            public int amount;

            [Tooltip("Specifies the input ID from which the item should be expected to enter. -1 for undefined")]
            public int inputID = -1;
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
            public int outputID;
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
