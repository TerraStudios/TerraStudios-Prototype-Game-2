using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TestTools;

namespace Assets.Tests
{
    public class ItemDataTests
    {
        /// <summary>
        /// Loops through each registered ItemData and checks different values to be valid
        /// </summary>
        [Test]
        public void CheckIfItemDataIsCorrect()
        {
            ItemData[] itemDB = Resources.LoadAll<ItemData>("");
            foreach (var item in itemDB)
            {
                Assert.IsNotEmpty(item.name, "An item was found with no name.");
                Assert.IsNotNull(item.obj, $"The item {item.name} had a null obj attached.");
                Assert.IsNotEmpty(item.description, $"The item ${item.name} was found with no description.");
                Assert.IsNotNull(item.obj.data, $"The prefab {item.obj.name} had a null ItemData attached.");
                Assert.IsNotNull(item.ItemCategory, $"An ItemCategory for ${item.name} has not been specified");
                Assert.Greater(item.startingPriceInShop, 0, $"The price for {item.name} has been found <= 0.");
            }
        }

    }
}

