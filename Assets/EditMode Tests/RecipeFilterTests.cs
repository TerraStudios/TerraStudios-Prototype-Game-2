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
