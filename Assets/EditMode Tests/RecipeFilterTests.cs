//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using NUnit.Framework;
using RecipeManagement;
using UnityEngine;

namespace Assets.Tests
{
    public class RecipeFilterTests
    {
        [Test]
        public void TestRecipeFilters_RecipeFilterValues_HasValidInputsAmount()
        {
            foreach (RecipeFilter r in Resources.LoadAll<RecipeFilter>(""))
            {
                if (r.enableAutomaticList && r.buildingInputsAmount == 0 && r.type == RecipeType.Allowed)
                    Assert.IsFalse(r.enableAutomaticList && r.buildingInputsAmount == 0 && r.type == RecipeType.Allowed, $"Possibly invalid recipe filter detected: {r.name}");
            }
        }

        [Test]
        public void TestRecipeFilters_RecipeFilterValues_HasValidOutputsAmount()
        {
            foreach (RecipeFilter r in Resources.LoadAll<RecipeFilter>(""))
            {
                Assert.IsFalse(r.buildingOutputsAmount == 0, $"Building Outputs Amount is 0 for recipe: {r.name}");
            }
        }

        [Test]
        public void TestRecipeFilterLists_RecipeFilterListValues_ContainsNullRecipe()
        {
            foreach (RecipeFilterList r in Resources.LoadAll<RecipeFilterList>(""))
            {
                Assert.IsFalse(r.recipes.Contains(null), $"RecipeFilterList {r.name} contains null recipe!");
            }
        }
    }
}
