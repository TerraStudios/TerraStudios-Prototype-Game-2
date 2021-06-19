//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System.Collections.Generic;
using BuildingModules;
using NUnit.Framework;
using RecipeManagement;
using UnityEngine;

namespace Tests
{
    public class AutoRecipeFilterTests
    {
        [Test]
        public void TestRecipeInAPM_AllowedRecipes_HasAtleastOneAllowedRecipe()
        {
            foreach (GameObject obj in Resources.LoadAll<GameObject>("Buildings"))
            {
                APM apm = obj.GetComponent<APM>();
                if (apm)
                {
                    RecipeFilter filter = apm.recipeFilter;
                    if (filter)
                    {
                        RecipeManager.LoadResources();
                        List<MachineRecipe> allowed = RecipeManager.GetRecipes(filter).allowed;

                        Assert.IsFalse(allowed.Count == 0, $"No allowed recipes found for recipe '{filter.name}' in APM '{obj.name}'");
                    }
                }
            }
        }

        [Test]
        public void TestRecipeInAPM_RecipeIO_HasCorrectRecipeIOSize()
        {
            foreach (GameObject obj in Resources.LoadAll<GameObject>("Buildings"))
            {
                APM apm = obj.GetComponent<APM>();
                BuildingIOManager io = obj.GetComponent<BuildingIOManager>();

                if (apm)
                {
                    RecipeFilter filter = apm.recipeFilter;
                    if (filter)
                    {
                        RecipeManager.LoadResources();

                        foreach (MachineRecipe recipe in RecipeManager.GetRecipes(filter).allowed)
                        {
                            Assert.IsFalse(io.inputs.Length < recipe.inputs.Count,
                                "Recipe inputs size is higher than APM inputs size. " +
                                $"APM '{obj.name}' has '{io.inputs.Length}' inputs while recipe '{recipe.name}' attached from the recipe filter '{filter.name}' requires '{recipe.inputs.Count}' inputs");

                            Assert.IsFalse(io.outputs.Length < recipe.outputs.Count,
                                "Recipe inputs size is higher than APM inputs size. " +
                                $"APM '{obj.name}' has '{io.outputs.Length}' inputs while recipe '{recipe.name}' attached from the recipe filter '{filter.name}' requires '{recipe.outputs.Count}' outputs");
                        }
                    }
                }
            }
        }
    }
}
