using UnityEngine;

[CreateAssetMenu(fileName = "New Item Category", menuName = "Item/Item Category")]
public class ItemCategory : ItemOrCategory
{
    [Tooltip("Formatted name for the category. Will be used in case formatted names are needed in the future.")]
    public string formattedName;
}
