using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Recipe", menuName = "ScriptableObjects/MachineRecipe", order = 1)]
public class MachineRecipe : ScriptableObject
{
    /// <summary>
    /// Represents a serializable dictionary containing the item and its amount 
    /// </summary>
    [Serializable]
    public class InputData
    {
        public ScriptableObject item;
        public int amount;
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

    [Tooltip("The base time (in seconds) for the recipe to finish processing.")]
    public float baseTime;

    [Tooltip("The icon for this recipe.")]
    public Texture2D icon;

}
