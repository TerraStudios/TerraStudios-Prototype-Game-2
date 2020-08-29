using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TestTools;

namespace Assets.Tests
{
    class CategoryTests
    {

        /// <summary>
        /// Loops through each registered Category and checks different values to be non empty
        /// </summary>
        [Test]
        public void CheckIfCategoryFieldsAreValid()
        {
            ItemCategory[] categories = Resources.LoadAll<ItemCategory>("");
            foreach (var category in categories)
            {
                Assert.IsNotEmpty(category.formattedName, $"The category {category.name} had a blank formatted name.");
                Assert.IsNotEmpty(category.name, "", $"The category {category.formattedName} had a blank name.");
            }
        }


    }
}
