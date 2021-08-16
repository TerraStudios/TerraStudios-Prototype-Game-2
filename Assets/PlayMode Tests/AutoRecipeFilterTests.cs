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
                            foreach (MachineRecipe.InputBatch batch in recipe.inputs)
                            {
                                foreach (MachineRecipe.InputData input in batch.inputs)
                                {
                                    int requiredInputs = input.inputId;
                                    Assert.IsFalse(io.inputs.Length < requiredInputs,
                                        "Recipe inputs size is higher than APM inputs size. " +
                                        $"APM '{obj.name}' has '{io.inputs.Length}' inputs while recipe '{recipe}' attached from the recipe filter '{filter.name}' requires '{requiredInputs}' inputs");
                                }
                            }

                            foreach (MachineRecipe.OutputBatch batch in recipe.outputs)
                            {
                                foreach (MachineRecipe.OutputData input in batch.outputs)
                                {
                                    int requiredOutputs = input.outputId;
                                    Assert.IsFalse(io.inputs.Length < requiredOutputs,
                                        "Recipe inputs size is higher than APM inputs size. " +
                                        $"APM '{obj.name}' has '{io.inputs.Length}' inputs while recipe '{recipe}' attached from the recipe filter '{filter.name}' requires '{requiredOutputs}' inputs");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
