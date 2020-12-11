using NUnit.Framework;
using RecipeManagement;
using UnityEngine;

namespace Assets.Tests
{
    public class RecipeTests
    {
        /// <summary>
        /// Loops through each registered MachineRecipe and checks different values to be valid
        /// </summary>
        [Test]
        public void TestRecipes_RecipeValues_HasCorrectRecipeValues()
        {
            foreach (MachineRecipe recipe in Resources.LoadAll<MachineRecipe>(""))
            {
                Assert.IsNotEmpty(recipe.name, $"A recipe was found with no name.");
                // TODO: UNCOMMENT THIS WHEN ICONS WILL ACTUALLY BE USED
                //Assert.IsNotNull(recipe.icon, $"The recipe ${recipe.name} was found with no icon.");
                Assert.AreNotEqual(recipe.baseTime, 0, $"The recipe {recipe.name} was found with a base time of 0 seconds.");
                Assert.AreNotEqual(recipe.inputs.Count, 0, $"The recipe {recipe.name} doesn't have any inputs assigned.");
                Assert.AreNotEqual(recipe.outputs.Count, 0, $"The recipe {recipe.name} doesn't have any outputs assigned.");
            }
        }

        [Test]
        public void TestRecipes_RecipeValues_HasValidOutputID()
        {
            foreach (MachineRecipe recipe in Resources.LoadAll<MachineRecipe>(""))
            {
                foreach (MachineRecipe.OutputBatch batch in recipe.outputs)
                {
                    foreach (MachineRecipe.OutputData data in batch.outputs)
                    {
                        Assert.IsFalse(data.outputID == -1, $"Recipe {recipe.name} has an OutputData with outputID = -1!");
                    }
                }
            }
        }

        [Test]
        public void TestRecipes_RecipeValues_HasValidBaseTimeValue()
        {
            foreach (MachineRecipe recipe in Resources.LoadAll<MachineRecipe>(""))
            {
                Assert.IsFalse(recipe.baseTime == 0, $"Recipe {recipe.name} has incorrect recipe time of 0!");
            }
        }
    }
}