using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Shell class only used for allowing a recipe to filter only ItemData or ItemCategory
///
/// - NOTE: This should not be used for ANYTHING other than UI display.
/// </summary>
public abstract class ItemTag : ScriptableObject
{

    public abstract bool Matches(ItemData item);
}

[CreateAssetMenu(fileName = "New Machine Recipe", menuName = "Recipe/Machine Recipe")]
public class MachineRecipe : ScriptableObject
{

    /// <summary>
    /// Represents a serializable dictionary containing the tag and its amount, as well as the input ID the item should be expected to enter.
    /// </summary>
    [Serializable]
    public class UnityInputData
    {
        public ItemTag item;
        public int amount;

        [Tooltip("Specifies the input ID from which the item should be expected to enter. -1 for undefined")]
        public int inputID = -1;
    }

    /// <summary>
    /// Represents a serializable dictionary containing the tag and its amount, as well as the input ID the item should be expected to enter.
    /// </summary>
    [Serializable]
    public class RecipeInputData
    {
        [Tooltip("Specifies the amount of items required for this recipe component")]
        public int amount;

        [Tooltip("Specifies the input ID from which the item should be expected to enter. -1 for wilcard")]
        public int inputID = -1;
    }



    /// <summary>
    /// Represents a serializable dictionary containing the item and its amount.
    /// </summary>
    [Serializable]
    public class OutputData
    {
        public ItemData item;
        public int amount;
    }

    /// <summary>
    /// Stores data relating to an <see cref="ItemTag"/>
    /// </summary>
    public struct ItemTagData
    {
        public ItemTag tag;
        public int amount;
    }

    [Header("Items")]

    [Tooltip("Specifies the input items and their corresponding amounts for the recipe.")]
    public UnityInputData[] _inputs;

    [Tooltip("Specifies the output items and their corresponding amounts for the recipe.")]
    public OutputData[] outputs;

    [Header("Other Info")]
    [Tooltip("The name of the recipe to display.")]
    public new string name;

    [Tooltip("The base time (in seconds) for the recipe to finish processing.")]
    public float baseTime;

    [Tooltip("The icon for this recipe.")]
    public Texture2D icon;

    //Dictionary<inputID, Dictionary<ItemTag, count>>
    private Dictionary<int, HashSet<ItemTagData>> _inputData;

    public Dictionary<int, HashSet<ItemTagData>> GetInputData()
    {

        if (_inputData != null)
        {
            return _inputData;
        }
        else
        {
            Dictionary<int, HashSet<ItemTagData>> inputData = new Dictionary<int, HashSet<ItemTagData>>();

            Array.ForEach(_inputs, input =>
            {

                HashSet<ItemTagData> set = inputData.GetOrPut(input.inputID);

                set.Add(new ItemTagData { tag = input.item, amount = input.amount });

            });

            _inputData = inputData;

            return inputData;
        }


    }

}
