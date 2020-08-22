using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Category", menuName = "Item/Item Category")]
public class ItemCategory : ItemTag
{
    [Tooltip("Formatted name for the category. Will be used in case formatted names are needed in the future.")]
    public string formattedName;

    [HideInInspector] public List<ItemData> items = new List<ItemData>();

    public override bool Matches(ItemData item)
    {
        return items.Any(thisItem => thisItem.ID == item.ID);
    }
}