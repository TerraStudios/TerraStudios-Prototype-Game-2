using UnityEngine;

[CreateAssetMenu(fileName = "ItemCategory", menuName = "ScriptableObjects/ItemCategory", order = 1)]
public class ItemCategory : ScriptableObject {

    [Tooltip("Formatted name for the category. Will be used in case formatted names are needed in the future.")]
    public string formattedName;
}