using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Machine Recipe", menuName = "Recipe/Machine Recipe")]
public class MachineRecipe : ScriptableObject
{
    /// <summary>
    /// Represents a serializable dictionary containing the item and its amount 
    /// </summary>
    [Serializable]
    public class InputData
    {
        public ItemOrCategory item;
        public int amount;

        [Tooltip("Specifies the input ID from which the item should be expected to enter. -1 for undefined")]
        public int inputID = -1;
    }

    /// <summary>
    /// Represents a serializable dictionary containing the item and its amount.
    /// Uses ItemData instead of generic SO because it's not possible to output a category
    /// </summary>
    [Serializable]
    public class OutputData
    {
        public ItemData item;
        public int amount;
    }

    [Header("Items")]

    [Tooltip("Specifies the input items and their corresponding amounts for the recipe.")]
    public InputData[] inputs;

    [Tooltip("Specifies the output items and their corresponding amounts for the recipe.")]
    public OutputData[] outputs;

    [Header("Other Info")]
    [Tooltip("The name of the recipe to display.")]
    public new string name;

    [Tooltip("The base time (in seconds) for the recipe to finish processing.")]
    public float baseTime;

    [Tooltip("The icon for this recipe.")]
    public Texture2D icon;

}
