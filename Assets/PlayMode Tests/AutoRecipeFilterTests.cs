using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public class AutoRecipeFilterTests
    {
        [Test]
        public void CheckNoAllowedRecipes()
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
        public void CheckRecipeInputFit()
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
                        }
                    }
                }
            }
        }
    }

}
