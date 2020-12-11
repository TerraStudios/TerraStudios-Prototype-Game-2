using ItemManagement;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Tests
{
    public class ItemDataTests
    {
        /// <summary>
        /// Loops through each registered ItemData and checks different values to be valid
        /// </summary>
        [Test]
        public void TestItemDatas_ItemDataValues_HasValidValues()
        {
            foreach (ItemData item in Resources.LoadAll<ItemData>(""))
            {
                Assert.IsNotEmpty(item.name, "An item was found with no name.");
                Assert.IsNotNull(item.obj, $"The item {item.name} had a null obj attached.");
                Assert.IsNotEmpty(item.description, $"The item ${item.name} was found with no description.");
                Assert.IsNotNull(item.obj.data, $"The prefab {item.obj.name} had a null ItemData attached.");
                Assert.Greater(item.startingPriceInShop, 0, $"The price for {item.name} has been found <= 0.");
            }
        }
    }
}
