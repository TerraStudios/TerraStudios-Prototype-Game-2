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
