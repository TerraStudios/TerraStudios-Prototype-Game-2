﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TestTools;

namespace Assets.Tests
{
    public class RecipeTests
    {
        /// <summary>
        /// Loops through each registered MachineRecipe and checks different values to be valid
        /// </summary>
        [Test]
        public void CheckForInvalidRecipes()
        {
            foreach (var recipe in Resources.LoadAll<MachineRecipe>(""))
            {
                Assert.IsNotEmpty(recipe.name, $"A recipe was found with no name.");
                // TODO: UNCOMMENT THIS WHEN ICONS WILL ACTUALLY BE USED
                //Assert.IsNotNull(recipe.icon, $"The recipe ${recipe.name} was found with no icon.");
                Assert.AreNotEqual(recipe.baseTime, 0, $"The recipe ${recipe.name} was found with a base time of 0 seconds.");
                Assert.AreNotEqual(recipe.inputs.Length, 0, $"The recipe {recipe.name} doesn't have any inputs assigned.");
                Assert.AreNotEqual(recipe.outputs.Length, 0, $"The recipe {recipe.name} doesn't have any outputs assigned.");
            }
        }

    }
}